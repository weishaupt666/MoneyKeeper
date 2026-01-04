using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoneyKeeper.DTO;
using MoneyKeeper.Integrations.Nbp.Interfaces;
using MoneyKeeper.Models;
using MoneyKeeper.Services;
using MoneyKeeper.Tests.Helpers;
using MoneyKeeper.Enums;
using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace MoneyKeeper.Tests.Services;

public class TransactionServiceTests
{
    public static IEnumerable<object[]> GetTransactionTestData()
    {
        yield return new object[] { 100m, "USD", 490m };
        yield return new object[] { 50m, "EUR", 225m };
        yield return new object[] { 10m, "USD", 38.2m };
    }

    [Theory]
    [MemberData(nameof(GetTransactionTestData))]
    public async Task CreateTransaction_ShouldConvertCurrency_WhenCurrencyIsNotPLN(
        decimal inputAmount,
        string currency,
        decimal expectedAmountInPln)
    {
        using var context = TestDbContextFactory.Create();

        var user = new User { Id = 1, Username = "TestUser", PasswordHash = "hash" };
        var wallet = new Wallet { Id = 1, UserId = 1, Name = "Main Wallet", Balance = 1000 };
        var category = new Category { Id = 1, Name = "Salary" };

        context.Users.Add(user);
        context.Wallets.Add(wallet);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var mockCurrencyService = new Mock<ICurrencyService>();

        mockCurrencyService
            .Setup(service => service.ConvertAsync(currency, "PLN", inputAmount))
            .ReturnsAsync(expectedAmountInPln);

        var transactionService = new TransactionService(context, mockCurrencyService.Object);

        var request = new CreateTransactionRequest
        {
            Amount = inputAmount,
            CurrencyCode = currency,
            WalletId = 1,
            CategoryId = 1,
            Type = OperationType.Income,
            Description = "Test",
            Date = DateTime.UtcNow
        };


        var result = await transactionService.CreateTransactionAsync(request, userId: 1);
        Assert.Equal(inputAmount, result.OriginalAmount);
        Assert.Equal(currency, result.OriginalCurrencyCode);
    }

    [Fact]
    public async Task CreateTransaction_ShouldThrowException_WhenBalanceIsInsufficient()
    {
        using var context = TestDbContextFactory.Create();

        var user = new User { Id = 1, Username = "TestUser", PasswordHash = "hash" };
        var wallet = new Wallet { Id = 1, UserId = 1, Name = "Poor Wallet", Balance = 10m, CurrencyCode = "PLN" };
        var category = new Category { Id = 1, Name = "Food" };

        context.Users.Add(user);
        context.Wallets.Add(wallet);
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var mockCurrencyService = new Mock<ICurrencyService>();

        mockCurrencyService
            .Setup(s => s.ConvertAsync("PLN", "PLN", 100m))
            .ReturnsAsync(100m);

        var transactionService = new TransactionService(context, mockCurrencyService.Object);

        var request = new CreateTransactionRequest
        {
            Amount = 100m,
            CurrencyCode = "PLN",
            WalletId = 1,
            CategoryId = 1,
            Type = OperationType.Expense,
            Description = "Too expensive",
            Date = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await transactionService.CreateTransactionAsync(request, userId: 1);
        });
    }

    [Fact]
    public async Task GetTransactions_ShouldFilterByDate_AndReturnOnlyRelevantRecords()
    {
        using var context = TestDbContextFactory.Create();

        var userId = 10;
        var user = new User { Id = userId, Username = "FilterUser", PasswordHash = "hash" };
        var wallet = new Wallet { Id = 5, UserId = userId, Name = "Test Wallet", Balance = 1000, CurrencyCode = "PLN" };
        var category = new Category { Id = 1, Name = "General" };

        context.Users.Add(user);
        context.Wallets.Add(wallet);
        context.Categories.Add(category);

        var dateNow = DateTime.UtcNow;

        var t1_Past = new Transaction
        {
            WalletId = wallet.Id,
            CategoryId = category.Id,
            Amount = 100,
            OriginalAmount = 100,
            OriginalCurrencyCode = "PLN",
            Date = dateNow.AddMonths(-1),
            Type = OperationType.Expense
        };

        var t2_Target = new Transaction
        {
            WalletId = wallet.Id,
            CategoryId = category.Id,
            Amount = 50,
            OriginalAmount = 50,
            OriginalCurrencyCode = "PLN",
            Date = dateNow,
            Type = OperationType.Expense
        };

        var t3_Future = new Transaction
        {
            WalletId = wallet.Id,
            CategoryId = category.Id,
            Amount = 200,
            OriginalAmount = 200,
            OriginalCurrencyCode = "PLN",
            Date = dateNow.AddMonths(1),
            Type = OperationType.Expense
        };

        context.Transactions.AddRange(t1_Past, t2_Target, t3_Future);
        await context.SaveChangesAsync();

        var service = new TransactionService(context, Mock.Of<ICurrencyService>());

        var filter = new GetTransactionsFilter
        {
            FromDate = dateNow.AddDays(-1),
            ToDate = dateNow.AddDays(1)
        };

        var result = await service.GetTransactionsAsync(filter, userId);

        Assert.Single(result);
        Assert.Equal(t2_Target.Amount, result[0].Amount);
    }

    [Fact]
    public async Task UpdateTransaction_ShouldCorrectlyUpdateBalance_WhenTypeChangedFromExpenseToIncome()
    {
        using var context = TestDbContextFactory.Create();

        var userId = 10;
        var user = new User { Id = userId, Username = "FilterUser", PasswordHash = "hash" };
        var wallet = new Wallet { Id = 5, UserId = userId, Name = "Test Wallet", Balance = 900 };
        var category = new Category { Id = 1, Name = "General" };
        var transaction = new Transaction
        {
            Id = 1,
            Amount = 100,
            OriginalAmount = 100,
            OriginalCurrencyCode = "PLN",
            Type = OperationType.Expense,
            WalletId = 5,
            CategoryId = 1,
            Date = DateTime.UtcNow,
            Wallet = wallet
        };

        context.Users.Add(user);
        context.Wallets.Add(wallet);
        context.Categories.Add(category);
        context.Transactions.Add(transaction);
        await context.SaveChangesAsync();


        var mockCurrencyService = new Mock<ICurrencyService>();
        mockCurrencyService
            .Setup(s => s.ConvertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
            .ReturnsAsync((string from, string to, decimal amount) => amount);

        var service = new TransactionService(context, mockCurrencyService.Object);

        var request = new UpdateTransactionRequest
        {
            Type = OperationType.Income,
            Amount = 100,
            CurrencyCode = "PLN",
            CategoryId = 1
        };

        await service.UpdateTransactionAsync(1, request, userId);

        var updatedWallet = await context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == 5);

        Assert.NotNull(updatedWallet);
        Assert.Equal(1100, wallet.Balance);

        var updatedTransaction = await context.Transactions.FindAsync(1);
        Assert.Equal(OperationType.Income, updatedTransaction!.Type);
    }
}
