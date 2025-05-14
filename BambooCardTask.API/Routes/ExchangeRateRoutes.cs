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
            baseCurrency ??= providerConfig.BaseCurrency ?? "EUR";
            logger.Information("{ProviderConfig} : {BaseCurrency}", providerConfig.Name, baseCurrency);
            var exchangeRates = await exchangeRateService.GetLatestExchangeRatesAsync(baseCurrency);
            return HandleServiceResult(exchangeRates, logger,
                failMessage: $"Failed to fetch exchange rates for base currency: {baseCurrency}",
                successMessage: $"Successfully retrieved exchange rates for base currency: {baseCurrency}",
                badRequestMessage: $"Invalid base currency: {baseCurrency}");
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
            var exchangeRates = await exchangeRateService.GetConversionRatesAsync(currencyConversionRequest);
            return HandleServiceResult(exchangeRates, logger,
                failMessage: $"Failed to fetch conversion rates for {currencyConversionRequest.FromCurrency}",
                successMessage: $"Successfully converted currency from {currencyConversionRequest.FromCurrency} to {string.Join(",", currencyConversionRequest.TargetCurrencies)}",
                problemMessage: "Failed to fetch exchange rates.");
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
            var historicalRates = await exchangeRateService.GetHistoricalExchangeRatesAsync(historicalExchangeRates);
            return HandleServiceResult(historicalRates, logger,
                failMessage: $"Failed to fetch historical exchange rates from {historicalExchangeRates.StartDate} to {historicalExchangeRates.EndDate} for base currency: {historicalExchangeRates.FromCurrency}",
                successMessage: $"Successfully fetched historical exchange rates from {historicalExchangeRates.StartDate} to {historicalExchangeRates.EndDate} for base currency: {historicalExchangeRates.FromCurrency}",
                problemMessage: "Failed to fetch historical exchange rates.");
        }).WithName("GetHistoricalExchangeRates");

    }

    // Helper method to handle null/error results and logging
    private static IResult HandleServiceResult<T>(T? result, Serilog.ILogger logger, string failMessage, string successMessage, string? badRequestMessage = null, string? problemMessage = null) where T : class
    {
        if (result == null)
        {
            logger.Warning(failMessage);
            if (!string.IsNullOrEmpty(badRequestMessage))
                return Results.BadRequest(badRequestMessage);
            if (!string.IsNullOrEmpty(problemMessage))
                return Results.Problem(problemMessage);
            return Results.Problem("An error occurred.");
        }
        logger.Information(successMessage);
        return Results.Ok(result);
    }
}