using MoneyKeeper.Enums;
using System.ComponentModel.DataAnnotations;

namespace MoneyKeeper.DTO;

public class UpdateTransactionRequest
{
    [Required]
    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; } = "PLN";
    public string? Description { get; set; }
    public OperationType? Type { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public DateTime Date { get; set; }
}
