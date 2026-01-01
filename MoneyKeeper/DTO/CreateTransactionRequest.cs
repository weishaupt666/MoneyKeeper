using MoneyKeeper.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MoneyKeeper.DTO;

public class CreateTransactionRequest
{
    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Amount must be greater than zero.")]
    public decimal? Amount { get; set; }
    [Required]
    public OperationType? Type { get; set; }
    [Required]
    public string CurrencyCode { get; set; } = "PLN";
    public string Description { get; set; } = string.Empty;
    [Required]
    public int? WalletId { get; set; }
    [Required]
    public int? CategoryId { get; set; }
    public DateTime Date { get; set; }
}
