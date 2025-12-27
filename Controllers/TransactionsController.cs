using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Data;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;
using MoneyKeeper.Enums;

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

        if (request.Type == OperationType.Income)
        {
            wallet.Balance += request.Amount!.Value;
        }
        else
        {
            if (wallet.Balance < (request.Amount ?? 0))
            {
                return BadRequest("Insufficient funds in wallet");
            }

            wallet.Balance -= request.Amount ?? 0;
        }


        var transaction = new Transaction
        {
            Amount = request.Amount ?? 0,
            Type = request.Type!.Value,
            WalletId = wallet.Id,
            Category = category,
            Description = request.Description ?? string.Empty,
            Date = request.Date == DateTime.MinValue ? DateTime.UtcNow : request.Date
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return Ok(transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        throw new Exception("Это тест глобального перехватчика!");
        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .ToListAsync();

        return Ok(transactions);
    }
}
