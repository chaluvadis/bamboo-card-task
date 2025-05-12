using BambooCardTask.Interfaces;
using BambooCardTask.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BambooCardTask.Services;

public class ExchangeRateService(IHttpClientFactory httpClientFactory) : IExchangeRateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ExchangeRateClient");

    public async ValueTask<ExchangeRates?> GetLatestExchangeRatesAsync(string baseCurrency)
    {
        // Use the HttpClient's base address
        var requestUrl = string.IsNullOrEmpty(baseCurrency)
            ? string.Empty
            : $"?baseCurrency={baseCurrency}";

        return await _httpClient.GetFromJsonAsync<ExchangeRates>(requestUrl);
    }
}
