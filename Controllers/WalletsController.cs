using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.Models;
using MoneyKeeper.DTO;
using Microsoft.EntityFrameworkCore;

namespace MoneyKeeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public WalletsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
    {
        var wallet = new Wallet
        {
            Name = request.Name,
            Balance = request.Balance,
            Currency = request.Currency
        };
        _context.Wallets.Add(wallet);
        await _context.SaveChangesAsync();

        return Ok(wallet);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var wallet = await _context.Wallets.FindAsync(id);

        if (wallet == null)
        {
            return NotFound();
        }

        return Ok(wallet);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var wallets = await _context.Wallets.ToListAsync();
        return Ok(wallets);
    }
}
