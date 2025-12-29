using Microsoft.Identity.Client;
using MoneyKeeper.DTO;
using MoneyKeeper.Enums;
using MoneyKeeper.Models;
using MoneyKeeper.Services;
using MoneyKeeper.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace MoneyKeeper.Tests;

public class TransactionServiceTests
{
    [Fact]
    public async Task CreateTransaction_Expense_ShouldDecreaseWalletBalance()
    {
        var context = DbContextHelper.GetInMemoryDbContext();

        var wallet = new Wallet { Id = 1, Name = "Test Wallet", Balance = 1000 };
        context.Wallets.Add(wallet);

        var category = new Category { Id = 1, Name = "Food" };
        context.Categories.Add(category);

        await context.SaveChangesAsync();

        var service = new TransactionService(context);

        var request = new CreateTransactionRequest
        {
            Amount = 100,
            Type = OperationType.Expense,
            Date = DateTime.Now,
            WalletId = 1,
            CategoryId = 1,
            Description = "Test"
        };

        await service.CreateTransactionAsync(request);

        var updatedWallet = await context.Wallets.FindAsync(1);
        Assert.Equal(900, updatedWallet.Balance);
    }

    [Fact]
    public async Task DeleteTransaction_Expense_ShouldRefundMoney()
    {
        var context = DbContextHelper.GetInMemoryDbContext();

        var wallet = new Wallet
        {
            Id = 1,
            Name = "Test wallet",
            Balance = 900
        };
        context.Wallets.Add(wallet);

        var category = new Category { Id = 1, Name = "Test category" };
        context.Categories.Add(category);

        var transaction = new Transaction { Id = 1, Amount = 100, Type = OperationType.Expense, WalletId = 1, CategoryId = 1 };
        context.Transactions.Add(transaction);

        await context.SaveChangesAsync();

        var service = new TransactionService(context);

        await service.DeleteTransactionAsync(1);

        var updatedWallet = await context.Wallets.FindAsync(1);
        Assert.Equal(1000, updatedWallet.Balance);
    }
}
