using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Services;

public interface IWalletService
{
    Task<Wallet> Create(CreateWalletRequest request);
    Task<Wallet> GetById(int id);
    Task<List<Wallet>> GetAll();
}
