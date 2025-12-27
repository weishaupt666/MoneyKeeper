using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;
using Microsoft.EntityFrameworkCore;

namespace MoneyKeeper.Services;

public class WalletService : IWalletService
{
    private readonly ApplicationDbContext _context;

    public WalletService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet> Create(CreateWalletRequest request)
    {
        var wallet = new Wallet
        {
            Name = request.Name,
            Balance = request.Balance,
            Currency = request.Currency
        };
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return wallet;
    }

    public async Task<Wallet> GetById(int id)
    {
        var wallet = await _context.Wallets.FindAsync(id);

        if (wallet == null)
        {
            throw new KeyNotFoundException("Wallet not found");
        }

        return wallet;
    }

    public async Task<List<Wallet>> GetAll()
    {
        var wallets = await _context.Wallets.ToListAsync();
        return wallets;
    }
}
