using System.ComponentModel.DataAnnotations;

namespace MoneyKeeper.DTO;

public class CreateCategoryRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
