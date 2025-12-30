using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.Models;
using MoneyKeeper.DTO;
using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MoneyKeeper.Extensions;

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
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest request)
    {
        var userId = User.GetUserId();

        var wallet = await _walletService.CreateWalletAsync(request, userId);

        return Ok(wallet);
    }

    [HttpGet]
    public async Task<IActionResult> GetWallets()
    {
        var userId = User.GetUserId();

        var wallets = await _walletService.GetWalletsAsync(userId);

        return Ok(wallets);
    }
}
