using System.ComponentModel.DataAnnotations;
namespace MoneyKeeper.DTO;

public class CreateWalletRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = string.Empty;
}
