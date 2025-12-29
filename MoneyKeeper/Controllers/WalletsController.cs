using Microsoft.AspNetCore.Mvc;
using MoneyKeeper.Data;
using MoneyKeeper.Models;
using MoneyKeeper.DTO;
using Microsoft.EntityFrameworkCore;
using MoneyKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdString))
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdString);

        var wallet = await _walletService.CreateWalletAsync(request, userId);

        return Ok(wallet);
    }

    [HttpGet]
    public async Task<IActionResult> GetWallets()
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userId = int.Parse(userIdString!);

        var wallets = await _walletService.GetWalletsAsync(userId);

        return Ok(wallets);
    }
}
