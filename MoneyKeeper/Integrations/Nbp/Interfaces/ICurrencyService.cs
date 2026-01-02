namespace MoneyKeeper.Integrations.Nbp.Interfaces;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string currencyCode);
    Task<decimal> ConvertAsync(string fromCurrency, string toCurrency, decimal amount);
}
