using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Enums;
using MoneyKeeper.Extensions;
using MoneyKeeper.Integrations.Nbp.Interfaces;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrencyService _currencyService;

    public TransactionService(ApplicationDbContext context, ICurrencyService  currencyService)
    {
        _context = context;
        _currencyService = currencyService;
    }

    public async Task<List<TransactionResponse>> GetTransactionsAsync(GetTransactionsFilter filter, int userId)
    {
        if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
        {
            throw new ArgumentException("Date 'From' cannot be greater than date 'To'");
        }

        var query = _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Include(t => t.Wallet)
            .Where(t => t.Wallet!.UserId == userId)
            .AsQueryable();

        if (filter.FromDate.HasValue)
        {
            query = query.Where(t => t.Date >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(t => t.Date <= filter.ToDate.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == filter.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            query = query.Where(t =>
            t.Description != null &&
            t.Description.ToLower().Contains(filter.SearchText.ToLower()));
        }

        query = query.OrderByDescending(t => t.Date);

        var transactions = await query.ToListAsync();

        return transactions
            .Select(t => t.ToResponse())
            .ToList();
    }

    public async Task<TransactionResponse> CreateTransactionAsync(CreateTransactionRequest request, int userId)
    {
        var wallet = await _context.Wallets.FindAsync(request.WalletId);
        if (wallet == null) throw new KeyNotFoundException("Wallet not found");
        if (wallet.UserId != userId) throw new UnauthorizedAccessException("Access denied");

        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null) throw new KeyNotFoundException("Category not found");

        decimal transactionAmount = request.Amount!.Value;
        decimal walletAmount = transactionAmount;
        decimal exchangeRate = 1.0m;

        if (!string.Equals(request.CurrencyCode, wallet.CurrencyCode, StringComparison.OrdinalIgnoreCase))
        {
            walletAmount = await _currencyService.ConvertAsync(
                fromCurrency: request.CurrencyCode,
                toCurrency: wallet.CurrencyCode,
                amount: transactionAmount
            );

            exchangeRate = walletAmount / transactionAmount;
        }

        OperationType type = request.Type!.Value;
        if (type == OperationType.Income)
        {
            wallet.Deposit(walletAmount);
        }
        else
        {
            wallet.Withdraw(walletAmount);
        }

        var transaction = new Transaction
        {
            OriginalAmount = transactionAmount,
            OriginalCurrencyCode = request.CurrencyCode,
            Amount = walletAmount,
            ExchangeRate = exchangeRate,
            Type = type,
            WalletId = wallet.Id,
            CategoryId = category.Id,
            Description = request.Description,
            Date = request.Date == DateTime.MinValue ? DateTime.UtcNow : request.Date
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction.ToResponse();
    }

    public async Task DeleteTransactionAsync(int id, int userId)
    {

        var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Where(w => w.Wallet!.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null) throw new KeyNotFoundException("Transaction not found.");

        if (transaction.Wallet != null)
        {
            if (transaction.Type == OperationType.Income)
            {
                transaction.Wallet.Withdraw(transaction.Amount);
            }
            else
            {
                transaction.Wallet.Deposit(transaction.Amount);
            }
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<TransactionResponse> UpdateTransactionAsync(int id, UpdateTransactionRequest request, int userId)
    {
        var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Where(t => t.Wallet!.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null) throw new KeyNotFoundException("Transaction not found");

        var wallet = transaction.Wallet;

        if (transaction.Type == OperationType.Income)
        {
            wallet!.Withdraw(transaction.Amount);
        }
        else
        {
            wallet!.Deposit(transaction.Amount);
        }

        if (request.CategoryId.HasValue)
        {
            transaction.CategoryId = request.CategoryId.Value;
        }

        if (request.Date.HasValue) transaction.Date = request.Date.Value;
        if (!string.IsNullOrEmpty(request.Description)) transaction.Description = request.Description;
        if (request.Type.HasValue) transaction.Type = request.Type.Value;

        bool amountChanged = request.Amount.HasValue;
        bool currencyChanged = !string.IsNullOrEmpty(request.CurrencyCode)
            && request.CurrencyCode != transaction.OriginalCurrencyCode;

        if (amountChanged || currencyChanged)
        {
            decimal newOriginalAmount = request.Amount ?? transaction.OriginalAmount;
            string newCurrencyCode = request.CurrencyCode ?? transaction.OriginalCurrencyCode;

            decimal newWalletAmount = await _currencyService.ConvertAsync(
                newCurrencyCode,
                wallet.CurrencyCode,
                newOriginalAmount
            );

            transaction.OriginalAmount = newOriginalAmount;
            transaction.OriginalCurrencyCode = newCurrencyCode;
            transaction.Amount = newWalletAmount;

            transaction.ExchangeRate = (newOriginalAmount == 0) ? 0 : (newWalletAmount / newOriginalAmount);
        }

        if (transaction.Type == OperationType.Income)
        {
            wallet.Deposit(transaction.Amount);
        }
        else
        {
            wallet.Withdraw(transaction.Amount);
        }

        await _context.SaveChangesAsync();
        return transaction.ToResponse();
    }

    public async Task<List<CategoryStatistics>> GetExpensesByCategoryAsync(GetTransactionsFilter filter, int userId)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Wallet)
            .Where(t => t.Wallet!.UserId == userId)
            .AsNoTracking()
            .Where(t => t.Type == OperationType.Expense)
            .AsQueryable();

        if (filter.FromDate.HasValue)
        {
            query = query.Where(t => t.Date >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(t => t.Date <= filter.ToDate.Value);
        }

        var stats = await query
            .GroupBy(t => new { CategoryName = t.Category!.Name, Currency = t.Wallet!.CurrencyCode })
            .Select(g => new CategoryStatistics
            {
                CategoryName = g.Key.CategoryName,
                CurrencyCode = g.Key.Currency,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .OrderBy(x => x.CurrencyCode)
            .ThenByDescending(x => x.TotalAmount)
            .ToListAsync();

        return stats;
    }

    public async Task<DashboardStatistics> GetDashboardStatisticsAsync(GetTransactionsFilter filter, int userId)
    {
        var query = _context.Transactions
            .Include(t => t.Wallet)
            .Where(t => t.Wallet!.UserId == userId)
            .AsNoTracking()
            .AsQueryable();

        if (filter.FromDate.HasValue)
        {
            query = query.Where(t => t.Date >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(t => t.Date <= filter.ToDate.Value);
        }

        if (filter.WalletId.HasValue)
        {
            query = query.Where(t => t.WalletId == filter.WalletId.Value);
        }

        var rawStats = await query
            .GroupBy(t => new { t.Wallet!.CurrencyCode, t.Type })
            .Select(g => new
            {
                Currency = g.Key.CurrencyCode,
                Type = g.Key.Type,
                Total = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        decimal totalIncomeInPln = 0;
        decimal totalExpenseInPln = 0;

        string targetCurrency = "PLN";

        foreach (var stat in rawStats)
        {
            decimal convertedAmount = await _currencyService.ConvertAsync(
                fromCurrency: stat.Currency,
                toCurrency: targetCurrency,
                amount: stat.Total
            );

            if (stat.Type == OperationType.Income)
            {
                totalIncomeInPln += convertedAmount;
            }
            else
            {
                totalExpenseInPln += convertedAmount;
            }
        }

        return new DashboardStatistics
        {
            TotalIncome = totalIncomeInPln,
            TotalExpense = totalExpenseInPln,
            Balance = totalIncomeInPln - totalExpenseInPln
        };
    }
}
