using MoneyKeeper.Enums;
using System.ComponentModel.DataAnnotations;

namespace MoneyKeeper.DTO;

public class UpdateTransactionRequest
{
    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Description { get; set; }
    public OperationType? Type { get; set; }
    public int? CategoryId { get; set; }
    public DateTime? Date { get; set; }
}
