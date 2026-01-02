using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace MoneyKeeper.Services;

public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _context;

    public WalletService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> CreateWalletAsync(CreateWalletRequest request, int userId)
    {
        var wallet = new Wallet
        {
            Name = request.Name,
            Balance = request.Balance,
            CurrencyCode = request.Currency,
            UserId = userId
        };
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return wallet;
    }

    public async Task<List<Wallet>> GetWalletsAsync(int userId)
    {
        var wallets = await _context.Wallets
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return wallets;
    }
}
