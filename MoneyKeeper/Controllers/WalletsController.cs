using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.Models;
using MoneyKeeper.DTO;
using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Services;
using Microsoft.AspNetCore.Authorization;

namespace MoneyKeeper.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWalletRequest request)
    {
        var wallet = await _walletService.Create(request);
        return Ok(wallet);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var wallet = await _walletService.GetById(id);
        return Ok(wallet);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var wallets = await _walletService.GetAll();
        return Ok(wallets);
    }
}
