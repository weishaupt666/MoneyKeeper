using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request);
    Task<List<Transaction>> GetTransactionsAsync(GetTransactionsFilter filter);
    Task DeleteTransactionAsync(int id);
    Task UpdateTransactionAsync(int id, UpdateTransactionRequest request);
    Task<List<CategoryStatistics>> GetExpensesByCategoryAsync(GetTransactionsFilter filter);
    Task<DashboardStatistics> GetDashboardStatisticsAsync(GetTransactionsFilter filter);
}
