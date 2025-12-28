namespace MoneyKeeper.Models;

public class GetTransactionsFilter
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? CategoryId { get; set; }
    public string? SearchText { get; set; }
    public int? WalletId { get; set; }
}
