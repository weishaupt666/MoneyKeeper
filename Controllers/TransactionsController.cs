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
}
