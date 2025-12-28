using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;
using MoneyKeeper.Services;

namespace MoneyKeeper.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var transaction = await _transactionService.CreateTransactionAsync(request);
        return Ok(transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTransactionsFilter filter)
    {
        var transactions = await _transactionService.GetTransactionsAsync(filter);
        return Ok(transactions);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _transactionService.DeleteTransactionAsync(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionRequest request)
    {
        await _transactionService.UpdateTransactionAsync(id, request);
        return Ok(new { Message = "Transaction updated" });
    }

    [HttpGet("stats/categories")]
    public async Task<IActionResult> GetCategoryStats([FromQuery] GetTransactionsFilter filter)
    {
        var stats = await _transactionService.GetExpensesByCategoryAsync(filter);
        return Ok(stats);
    }
}
