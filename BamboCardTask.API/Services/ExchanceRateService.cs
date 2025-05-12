using BambooCardTask.Interfaces;
using BambooCardTask.Models;

namespace BambooCardTask.Services;

public class ExchangeRateService(IHttpClientFactory httpClientFactory) : IExchangeRateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ExchangeRateClient");

    public async ValueTask<ConversionRatesResponse?> GetConversionRatesAsync(CurrencyConversionRequest currencyConversionRequest)
    {
        // Use the HttpClient's base address
        // https://api.frankfurter.dev/v1/1999-01-04?base=EUR&symbols=USD,CAD,AUD
        var requestUrl = string.IsNullOrEmpty(currencyConversionRequest.FromCurrency)
            ? string.Empty
            : $"latest?base={currencyConversionRequest.FromCurrency}&symbols={string.Join(",", currencyConversionRequest.TargetCurrencies)}";
        return await _httpClient.GetFromJsonAsync<ConversionRatesResponse>(requestUrl);
    }

    public async ValueTask<ExchangeRatesResponse?> GetLatestExchangeRatesAsync(string baseCurrency)
    {
        // Use the HttpClient's base address
        var requestUrl = string.IsNullOrEmpty(baseCurrency)
            ? string.Empty
            : $"latest?base={baseCurrency}";

        return await _httpClient.GetFromJsonAsync<ExchangeRatesResponse>(requestUrl);
    }

    public async ValueTask<HistoricalExchangeRatesResponse> GetHistoricalExchangeRatesAsync(HistoricalExchangeRatesRequest historicalExchangeRates)
    {
        // Construct the request URL for the historical exchange rates API
        var requestUrl = $"{historicalExchangeRates.StartDate:yyyy-MM-dd}..{historicalExchangeRates.EndDate:yyyy-MM-dd}?base={historicalExchangeRates.FromCurrency}";

        // Fetch the data from the API
        return await _httpClient.GetFromJsonAsync<HistoricalExchangeRatesResponse>(requestUrl);
    }
}
