namespace BambooCardTask.Services;

public class ExchangeRateService(
    IHttpClientFactory httpClientFactory,
    ILogger<ExchangeRateService> logger
    ) : IExchangeRateService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ExchangeRateClient");
    private readonly ILogger<ExchangeRateService> _logger = logger;

    public async ValueTask<ExchangeRatesResponse?> GetLatestExchangeRatesAsync(string? baseCurrency)
    {
        _logger.LogInformation("Fetching latest exchange rates for base currency: {BaseCurrency}", baseCurrency);

        var requestUrl = string.IsNullOrEmpty(baseCurrency)
            ? string.Empty
            : $"latest?base={baseCurrency}";

        _logger.LogInformation("GetLatestExchangeRatesAsync: {requestUrl}", requestUrl);

        return await HandleServiceCall<ExchangeRatesResponse>(
            requestUrl: requestUrl,
            exContext: baseCurrency,
            contextType: "exchange rates"
        );
    }


    public async ValueTask<ConversionRatesResponse?> GetConversionRatesAsync(CurrencyConversionRequest currencyConversionRequest)
    {
        _logger.LogInformation("Fetching conversion rates for {FromCurrency} to {TargetCurrencies}",
                currencyConversionRequest.FromCurrency, string.Join(",", currencyConversionRequest.TargetCurrencies));
        var requestUrl = $"latest?base={currencyConversionRequest.FromCurrency}&symbols={string.Join(",", currencyConversionRequest.TargetCurrencies)}";

        _logger.LogInformation("GetConversionRatesAsync: {requestUrl}", requestUrl);

        return await HandleServiceCall<ConversionRatesResponse>(
            requestUrl: requestUrl,
            exContext: currencyConversionRequest.FromCurrency,
            contextType: "conversion rates"
        );
    }


    public async ValueTask<HistoricalExchangeRatesResponse?> GetHistoricalExchangeRatesAsync(HistoricalExchangeRatesRequest historicalExchangeRates)
    {
        _logger.LogInformation("Fetching historical exchange rates from {StartDate} to {EndDate} for base currency: {BaseCurrency}", historicalExchangeRates.StartDate, historicalExchangeRates.EndDate, historicalExchangeRates.FromCurrency);
        var requestUrl = $"{historicalExchangeRates.StartDate:yyyy-MM-dd}..{historicalExchangeRates.EndDate:yyyy-MM-dd}?base={historicalExchangeRates.FromCurrency}";
        _logger.LogInformation("GetHistoricalExchangeRatesAsync: {requestUrl}", requestUrl);
        return await HandleServiceCall<HistoricalExchangeRatesResponse>(
            requestUrl: requestUrl,
            exContext: historicalExchangeRates.FromCurrency,
            contextType: "historical exchange rates"
        );
    }

    // Shared handler for all service calls to avoid duplicate try/catch
    private async ValueTask<T?> HandleServiceCall<T>(string requestUrl, string? exContext, string contextType) where T : class
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<T>(requestUrl);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, $"HTTP error while fetching {contextType} for context: {{Context}}", exContext);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, $"Deserialization error while fetching {contextType} for context: {{Context}}", exContext);
            return null;
        }
    }
}
