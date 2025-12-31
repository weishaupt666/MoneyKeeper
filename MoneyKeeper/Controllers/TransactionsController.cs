using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.DTO;
using MoneyKeeper.Models;
using MoneyKeeper.Services;
using MoneyKeeper.Extensions;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using MoneyKeeper.Integrations.Nbp.Interfaces;

namespace MoneyKeeper.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ICurrencyService _currencyService;

    public TransactionsController(ITransactionService transactionService, ICurrencyService currencyService)
    {
        _transactionService = transactionService;
        _currencyService = currencyService;
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
    {
        var userId = User.GetUserId();
        var transaction = await _transactionService.CreateTransactionAsync(request, userId);
        return Ok(transaction);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetTransactionsFilter filter)
    {
        var userId = User.GetUserId();
        var transactions = await _transactionService.GetTransactionsAsync(filter, userId);
        return Ok(transactions);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        await _transactionService.DeleteTransactionAsync(id, userId);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionRequest request)
    {
        var userId = User.GetUserId();
        await _transactionService.UpdateTransactionAsync(id, request, userId);
        return Ok(new { Message = "Transaction updated" });
    }

    [HttpGet("stats/categories")]
    public async Task<IActionResult> GetCategoryStats([FromQuery] GetTransactionsFilter filter)
    {
        var userId = User.GetUserId();
        var stats = await _transactionService.GetExpensesByCategoryAsync(filter, userId);
        return Ok(stats);
    }

    [HttpGet("stats/dashboard")]
    public async Task<IActionResult> GetDashboardStats([FromQuery] GetTransactionsFilter filter)
    {
        var userId = User.GetUserId();
        var stats = await _transactionService.GetDashboardStatisticsAsync(filter, userId);
        return Ok(stats);
    }
}
