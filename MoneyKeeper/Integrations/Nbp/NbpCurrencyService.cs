using MoneyKeeper.Integrations.Nbp.Interfaces;
using MoneyKeeper.Integrations.Nbp.Models;
using MoneyKeeper.Services;
using System.Diagnostics;
using System.Net.Http.Json;

namespace MoneyKeeper.Integrations.Nbp;


public class NbpCurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    public NbpCurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetExchangeRateAsync(string currencyCode)
    {
        if (currencyCode.Equals("PLN", StringComparison.OrdinalIgnoreCase))
            return 1.0m;

        var url = $"http://api.nbp.pl/api/exchangerates/rates/a/{currencyCode.ToLower()}/?format=json";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<NbpApiResponse>(url);

            if (response == null || response.Rates.Count == 0)
            {
                throw new InvalidOperationException($"NBP API returned no rates for {currencyCode}");
            }

            return response.Rates[0].Mid;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Currency '{currencyCode}' not found in NBP.", ex);
            }

            throw new InvalidOperationException($"Failed to fetch rates for {currencyCode}", ex);
        }
    }

    public async Task<decimal> ConvertAsync(string fromCurrency, string toCurrency, decimal amount)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
        {
            return amount;
        }

        if (string.Equals(toCurrency, "PLN", StringComparison.OrdinalIgnoreCase))
        {
            var rate = await GetExchangeRateAsync(fromCurrency);
            return amount * rate;
        }

        if (string.Equals(fromCurrency, "PLN", StringComparison.OrdinalIgnoreCase))
        {
            var rate = await GetExchangeRateAsync(toCurrency);
            return amount / rate;
        }

        var fromRateTask = GetExchangeRateAsync(fromCurrency);
        var toRateTask = GetExchangeRateAsync(toCurrency);


        await Task.WhenAll(fromRateTask, toRateTask);

        var fromRate = fromRateTask.Result;
        var toRate = toRateTask.Result;

        return (amount * fromRate) / toRate;
    }
}
