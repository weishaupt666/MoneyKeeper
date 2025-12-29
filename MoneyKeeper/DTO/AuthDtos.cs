using System.ComponentModel.DataAnnotations;

namespace MoneyKeeper.DTO;

public class UserRegisterRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class UserLoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
