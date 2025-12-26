using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;

namespace MoneyKeeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public TransactionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var wallet = await _context.Wallets.FindAsync(request.WalletId);
        if (wallet == null)
        {
            return NotFound("Wallet not found");
        }

        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
        {
            return NotFound("Category not found");
        }

        wallet.Balance -= request.Amount ?? 0;

        var transaction = new Transaction
        {
            Amount = request.Amount ?? 0,
            WalletId = wallet.Id,
            Category = category,
            Date = request.Date == DateTime.MinValue ? DateTime.UtcNow : request.Date
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(transaction);
    }
}
