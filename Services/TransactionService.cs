using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Enums;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public class TransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;

    public TransactionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Transaction>> GetTransactionsAsync(GetTransactionsFilter filter)
    {
        if (filter.FromDate.HasValue && filter.ToDate.HasValue && filter.FromDate > filter.ToDate)
        {
            throw new ArgumentException("Date 'From' cannot be greater than date 'To'");
        }

        var query = _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
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

    public async Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request)
    {
        var wallet = await _context.Wallets.FindAsync(request.WalletId);
        if (wallet == null)
        {
            throw new KeyNotFoundException("Wallet not found");
        }

        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
        {
            throw new KeyNotFoundException("Category not found");
        }

        decimal amount = request.Amount!.Value;
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
}
