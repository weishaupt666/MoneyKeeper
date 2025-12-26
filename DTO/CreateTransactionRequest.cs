using MoneyKeeper.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MoneyKeeper.DTO;

public class CreateTransactionRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal? Amount { get; set; }
    [Required]
    public OperationType? Type { get; set; }
    public string Description { get; set; } = string.Empty;
    [Required]
    public int? WalletId { get; set; }
    [Required]
    public int? CategoryId { get; set; }
    public DateTime Date { get; set; }
}
