using MoneyKeeper.DTO;
using MoneyKeeper.Models;
using System.Runtime.CompilerServices;

namespace MoneyKeeper.Extensions;

public static class TransactionMappingExtensions
{
    public static TransactionResponse ToResponse(this Transaction t)
    {
        return new TransactionResponse
        {
            Id = t.Id,
            Amount = t.Amount,
            CurrencyCode = t.Wallet?.CurrencyCode ?? "PLN",
            OriginalAmount = t.OriginalAmount,
            OriginalCurrencyCode = t.OriginalCurrencyCode,
            ExchangeRate = t.ExchangeRate,
            Description = t.Description,
            Date = t.Date,
            Type = t.Type.ToString(),
            CategoryName = t.Category?.Name ?? "No Category",
            WalletName = t.Wallet?.Name ?? "No Wallet"
        };
    }
}
