using System.ComponentModel.DataAnnotations;
namespace MoneyKeeper.DTO;

public class CreateWalletRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    [Required]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Currency code must be 3 characters (e.g. USD).")]
    public string Currency { get; set; } = string.Empty;
}
