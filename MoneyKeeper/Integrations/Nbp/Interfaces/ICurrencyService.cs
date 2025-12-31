namespace MoneyKeeper.Integrations.Nbp.Interfaces;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string currencyCode);
}
