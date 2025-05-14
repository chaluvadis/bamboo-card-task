namespace BambooCardTask.Routes;
public static class ExchangeRateRoutes
{
    public static void MapExchangeRateRoutes(this WebApplication app)
    {
        // Add endpoint to retrieve latest exchange rates
        app.MapGet("api/exchange-rates/latest",
        async (
            IExchangeRateService exchangeRateService,
            IConfiguration configuration,
            [FromServices] Serilog.ILogger logger, // Explicitly use Serilog.ILogger
            string? baseCurrency
        ) =>
        {
            logger.Information("Retrieving latest exchange rates for base currency: {BaseCurrency}", baseCurrency);

            // Use baseCurrency from appsettings.json if not provided
            baseCurrency ??= configuration["CurrencyExchange:BaseCurrency"] ?? "EUR";

            // Fetch data from the service
            var exchangeRates = await exchangeRateService.GetLatestExchangeRatesAsync(baseCurrency);

            if (exchangeRates == null)
            {
                logger.Warning("Failed to fetch exchange rates for base currency: {BaseCurrency}", baseCurrency);
                return Results.Problem("Failed to fetch exchange rates from the API.");
            }

            logger.Information("Successfully retrieved exchange rates for base currency: {BaseCurrency}", baseCurrency);
            return Results.Ok(exchangeRates);
        }).WithName("GetLatestExchangeRates");

        // Add endpoint for currency conversion
        app.MapPost("api/exchange-rates/convert",
        async (
            IExchangeRateService exchangeRateService,
            [FromServices] Serilog.ILogger logger, // Explicitly use Serilog.ILogger
            CurrencyConversionRequest currencyConversionRequest
        ) =>
        {
            logger.Information("Converting currency from {FromCurrency} to {TargetCurrencies}", currencyConversionRequest.FromCurrency, string.Join(",", currencyConversionRequest.TargetCurrencies));

            // Fetch exchange rates
            var exchangeRates = await exchangeRateService.GetConversionRatesAsync(currencyConversionRequest);

            if (exchangeRates == null)
            {
                logger.Warning("Failed to fetch conversion rates for {FromCurrency}", currencyConversionRequest.FromCurrency);
                return Results.Problem("Failed to fetch exchange rates.");
            }

            logger.Information("Successfully converted currency from {FromCurrency} to {TargetCurrencies}", currencyConversionRequest.FromCurrency, string.Join(",", currencyConversionRequest.TargetCurrencies));
            return Results.Ok(exchangeRates);
        }).WithName("ConvertCurrency");

        // Add endpoint for historical exchange rates with pagination
        app.MapPost("api/exchange-rates/historical",
        async (
            IExchangeRateService exchangeRateService,
            [FromServices] Serilog.ILogger logger, // Explicitly use Serilog.ILogger
            [FromBody] HistoricalExchangeRatesRequest historicalExchangeRates
        ) =>
        {
            logger.Information("Fetching historical exchange rates from {StartDate} to {EndDate} for base currency: {BaseCurrency}", historicalExchangeRates.StartDate, historicalExchangeRates.EndDate, historicalExchangeRates.FromCurrency);

            // Fetch historical exchange rates
            var historicalRates = await exchangeRateService.GetHistoricalExchangeRatesAsync(historicalExchangeRates);

            if (historicalRates == null)
            {
                logger.Warning("Failed to fetch historical exchange rates from {StartDate} to {EndDate} for base currency: {BaseCurrency}", historicalExchangeRates.StartDate, historicalExchangeRates.EndDate, historicalExchangeRates.FromCurrency);
                return Results.Problem("Failed to fetch historical exchange rates.");
            }

            logger.Information("Successfully fetched historical exchange rates from {StartDate} to {EndDate} for base currency: {BaseCurrency}", historicalExchangeRates.StartDate, historicalExchangeRates.EndDate, historicalExchangeRates.FromCurrency);

            return Results.Ok(historicalRates);
        }).WithName("GetHistoricalExchangeRates");
    }
}
