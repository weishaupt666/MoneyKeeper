namespace MoneyKeeper.DTO;

public class TransactionResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public string OriginalCurrnecyCode { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string WalletName { get; set; } = string.Empty;
}
