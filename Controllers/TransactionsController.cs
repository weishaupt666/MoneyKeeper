using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.DTO;
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
    public async Task<IActionResult> GetAll()
    {
        var transactions = await _transactionService.GetTransactionsAsync();
        return Ok(transactions);
    }
}
