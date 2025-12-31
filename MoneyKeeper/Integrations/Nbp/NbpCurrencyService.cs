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
}
