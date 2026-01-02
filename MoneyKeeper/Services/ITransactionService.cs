using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request, int userId);
    Task<List<TransactionResponse>> GetTransactionsAsync(GetTransactionsFilter filter, int userId);
    Task DeleteTransactionAsync(int id, int userId);
    Task UpdateTransactionAsync(int id, UpdateTransactionRequest request, int userId);
    Task<List<CategoryStatistics>> GetExpensesByCategoryAsync(GetTransactionsFilter filter, int userId);
    Task<DashboardStatistics> GetDashboardStatisticsAsync(GetTransactionsFilter filter, int userId);
}
