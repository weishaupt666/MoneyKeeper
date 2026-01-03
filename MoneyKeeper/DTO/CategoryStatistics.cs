namespace MoneyKeeper.DTO;

public class CategoryStatistics
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
}
