namespace BambooCardTask.Services;

public class ExchangeRateService(
    IHttpClientFactory httpClientFactory,
    ILogger<ExchangeRateService> logger
    ) : IExchangeRateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ExchangeRateClient");
    private readonly ILogger<ExchangeRateService> _logger = logger;

    public async ValueTask<ConversionRatesResponse?> GetConversionRatesAsync(CurrencyConversionRequest currencyConversionRequest)
    {
        _logger.LogInformation("Fetching conversion rates for {FromCurrency} to {TargetCurrencies}",
                currencyConversionRequest.FromCurrency, string.Join(",", currencyConversionRequest.TargetCurrencies));

        var requestUrl = string.IsNullOrEmpty(currencyConversionRequest.FromCurrency)
            ? string.Empty
            : $"latest?base={currencyConversionRequest.FromCurrency}&symbols={string.Join(",", currencyConversionRequest.TargetCurrencies)}";

        _logger.LogInformation("GetConversionRatesAsync: {requestUrl}", requestUrl);

        return await _httpClient.GetFromJsonAsync<ConversionRatesResponse>(requestUrl);
    }

    public async ValueTask<ExchangeRatesResponse?> GetLatestExchangeRatesAsync(string baseCurrency)
    {
        _logger.LogInformation("Fetching latest exchange rates for base currency: {BaseCurrency}", baseCurrency);

        var requestUrl = string.IsNullOrEmpty(baseCurrency)
            ? string.Empty
            : $"latest?base={baseCurrency}";
        _logger.LogInformation("GetLatestExchangeRatesAsync: {requestUrl}", requestUrl);

        try
        {
            var response = await _httpClient.GetAsync(requestUrl);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("External API returned 404 for base currency: {BaseCurrency}", baseCurrency);
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ExchangeRatesResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error fetching exchange rates for base currency: {BaseCurrency}", baseCurrency);
            return null;
        }
    }

    public async ValueTask<HistoricalExchangeRatesResponse?> GetHistoricalExchangeRatesAsync(HistoricalExchangeRatesRequest historicalExchangeRates)
    {
        _logger.LogInformation("Fetching historical exchange rates from {StartDate} to {EndDate} for base currency: {BaseCurrency}", historicalExchangeRates.StartDate, historicalExchangeRates.EndDate, historicalExchangeRates.FromCurrency);

        var requestUrl = $"{historicalExchangeRates.StartDate:yyyy-MM-dd}..{historicalExchangeRates.EndDate:yyyy-MM-dd}?base={historicalExchangeRates.FromCurrency}";

        _logger.LogInformation("GetHistoricalExchangeRatesAsync: {requestUrl}", requestUrl);

        return await _httpClient.GetFromJsonAsync<HistoricalExchangeRatesResponse?>(requestUrl);
    }
}
