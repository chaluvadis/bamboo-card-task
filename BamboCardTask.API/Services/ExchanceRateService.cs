using BambooCardTask.Interfaces;
using BambooCardTask.Models;

namespace BambooCardTask.Services;

public class ExchangeRateService(IHttpClientFactory httpClientFactory) : IExchangeRateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ExchangeRateClient");

    public async ValueTask<ConversionRates?> GetConversionRatesAsync(CurrencyConversionRequest currencyConversionRequest)
    {
        // Use the HttpClient's base address
        // https://api.frankfurter.dev/v1/1999-01-04?base=EUR&symbols=USD,CAD,AUD
        var requestUrl = string.IsNullOrEmpty(currencyConversionRequest.FromCurrency)
            ? string.Empty
            : $"?base={currencyConversionRequest.FromCurrency}&symbols={string.Join(",", currencyConversionRequest.TargetCurrencies)}";
        return await _httpClient.GetFromJsonAsync<ConversionRates>(requestUrl);
    }

    public async ValueTask<ExchangeRates?> GetLatestExchangeRatesAsync(string baseCurrency)
    {
        // Use the HttpClient's base address
        var requestUrl = string.IsNullOrEmpty(baseCurrency)
            ? string.Empty
            : $"?base={baseCurrency}";

        return await _httpClient.GetFromJsonAsync<ExchangeRates>(requestUrl);
    }
}
