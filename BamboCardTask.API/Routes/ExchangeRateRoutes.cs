namespace BambooCardTask.Routes;
public static class ExchangeRateRoutes
{
    public static void MapExchangeRateRoutes(this WebApplication app)
    {

        var group = app.MapGroup("api/exchange-rates")
            .AddEndpointFilter<ExceptionFilter>()
            .WithTags("Exchange Rates");

        // Add endpoint to retrieve latest exchange rates
        group.MapGet("latest",
        [Authorize(Policy = "UserOnly")]
        async (
            IExchangeRateService exchangeRateService,
            [FromServices] ProviderConfig providerConfig,
            [FromServices] Serilog.ILogger logger,
            [FromQuery] string? baseCurrency
        ) =>
        {
            // If baseCurrency is not provided, use the one from providerConfig
            baseCurrency ??= providerConfig.BaseCurrency ?? "EUR";

            logger.Information("{ProviderConfig} : {BaseCurrency}", providerConfig.Name, baseCurrency);

            // Fetch data from the service
            var exchangeRates = await exchangeRateService.GetLatestExchangeRatesAsync(baseCurrency);

            if (exchangeRates == null)
            {
                logger.Warning("Failed to fetch exchange rates for base currency: {BaseCurrency}", baseCurrency);
                return Results.BadRequest($"Invalid base currency: {baseCurrency}");
            }

            logger.Information("Successfully retrieved exchange rates for base currency: {BaseCurrency}", baseCurrency);

            return Results.Ok(exchangeRates);
        }).WithName("GetLatestExchangeRates");

        // Add endpoint for currency conversion
        group.MapPost("convert",
        [Authorize(Policy = "UserOnly")]
        async (
            IExchangeRateService exchangeRateService,
            [FromServices] Serilog.ILogger logger,
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
        group.MapPost("historical",
        [Authorize(Policy = "AdminOnly")]
        async (
            IExchangeRateService exchangeRateService,
            [FromServices] Serilog.ILogger logger,
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
