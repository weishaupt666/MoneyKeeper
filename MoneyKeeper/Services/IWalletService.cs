using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface IWalletService
{
    Task<Wallet> CreateWalletAsync(CreateWalletRequest request, int userId);
    Task<List<Wallet>> GetWalletsAsync(int userId);
}
