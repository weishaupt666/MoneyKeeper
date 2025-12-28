using System.ComponentModel.DataAnnotations;

namespace MoneyKeeper.DTO;

public class UpdateTransactionRequest
{
    [Required]
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public DateTime Date { get; set; }
}
