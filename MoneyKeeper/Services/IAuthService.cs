using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface IAuthService
{
    Task RegisterAsync(UserRegisterRequest request);
    Task<User> LoginAsync(UserLoginRequest request);
}
