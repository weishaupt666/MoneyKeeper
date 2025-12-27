using MoneyKeeper.DTO;
using MoneyKeeper.Enums;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface ITransactionService
{
    Task<Transaction> CreateTransactionAsync(CreateTransactionRequest request);
    Task<List<Transaction>> GetTransactionsAsync();
}
