using MoneyKeeper.Enums;

namespace MoneyKeeper.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public decimal OriginalAmount { get; set; }
    public string OriginalCurrencyCode { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public DateTime Date { get; set; }
    public OperationType Type { get; set; }
    public string? Description { get; set; }
    public int WalletId { get; set; }
    public Wallet? Wallet { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
