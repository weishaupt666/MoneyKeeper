using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Enums;
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

    public async Task<List<Transaction>> GetTransactionsAsync(GetTransactionsFilter filter, int userId)
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

        return await query.ToListAsync();
    }

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request, int userId)
    {
        var wallet = await _context.Wallets.FindAsync(request.WalletId);
        if (wallet == null)
        {
            throw new KeyNotFoundException("Wallet not found");
        }

        if (wallet.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not own this wallet");
        }

        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        decimal amount = request.Amount!.Value;

        if (request.CurrencyCode.ToUpper() != "PLN")
        {
            decimal rate = await _currencyService.GetExchangeRateAsync(request.CurrencyCode);
            amount *= rate;
            request.Description += $"(Converted from {request.Amount} {request.CurrencyCode} at rate {rate})";
        }
        OperationType type = request.Type!.Value;

        if (type == OperationType.Income)
        {
            wallet.Balance += amount;
        }
        else
        {
            if (wallet.Balance < amount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }
            wallet.Balance -= amount;
        }

        var transaction = new Transaction
        {
            Amount = amount,
            Type = type,
            WalletId = wallet.Id,
            Category = category,
            Description = request.Description ?? string.Empty,
            Date = request.Date == DateTime.MinValue ? DateTime.UtcNow : request.Date
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task DeleteTransactionAsync(int id, int userId)
    {

        var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Where(w => w.Wallet!.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
        {
            throw new KeyNotFoundException("Transaction not found");
        }

        var wallet = transaction.Wallet;

        if (transaction.Type == OperationType.Income)
        {
            if (wallet!.Balance < transaction.Amount)
            {
                throw new InvalidOperationException("Cannot delete income: insufficient funds to rollback");
            }
            wallet.Balance -= transaction.Amount;
        }
        else
        {
            wallet!.Balance += transaction.Amount;
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTransactionAsync(int id, UpdateTransactionRequest request, int userId)
    {
        var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Where(t => t.Wallet!.UserId == userId)
                .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
        {
            throw new KeyNotFoundException("Transaction not found");
        }

        var wallet = transaction.Wallet;

        if (transaction.Type == OperationType.Income)
        {
            if (wallet!.Balance < transaction.Amount)
            {
                throw new InvalidOperationException("Cannot rollback income: insufficient funds");
            }

            wallet!.Balance -= transaction.Amount;
        }
        else
        {
            wallet!.Balance += transaction.Amount;
        }

        decimal finalAmount = request.Amount ?? transaction.Amount;
        string currency = string.IsNullOrEmpty(request.CurrencyCode) ? "PLN" : request.CurrencyCode.ToUpper();

        if (currency != "PLN")
        {
            decimal rate = await _currencyService.GetExchangeRateAsync(currency);
            finalAmount *= rate;
        }

        if (request.Type.HasValue)
        {
            transaction.Type = request.Type.Value;
        }

        transaction.Amount = finalAmount;
        transaction.Description = request.Description ?? transaction.Description;
        transaction.Date = request.Date;
        transaction.CategoryId = request.CategoryId;

        if (transaction.Type == OperationType.Income)
        {
            wallet.Balance += transaction.Amount;
        }
        else
        {
            if (wallet.Balance < transaction.Amount)
            {
                throw new InvalidOperationException("Insufficient funds for update amount.");
            }
            wallet.Balance -= transaction.Amount;
        }

        await _context.SaveChangesAsync();
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
            .GroupBy(t => t.Category!.Name)
            .Select(g => new CategoryStatistics
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            })
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

        var totalIncome = await query
            .Where(t => t.Type == OperationType.Income)
            .SumAsync(t => t.Amount);

        var totalExpense = await query
            .Where(t => t.Type == OperationType.Expense)
            .SumAsync(t => t.Amount);

        return new DashboardStatistics
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = totalIncome - totalExpense
        };
    }
}
