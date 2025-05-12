using BambooCardTask.Interfaces;
using BambooCardTask.Models;

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
            string? baseCurrency
        ) =>
        {
            // Use baseCurrency from appsettings.json if not provided
            baseCurrency ??= configuration["CurrencyExchange:BaseCurrency"] ?? "EUR";

            // Fetch data from the service
            var exchangeRates = await exchangeRateService.GetLatestExchangeRatesAsync(baseCurrency);

            if (exchangeRates == null)
            {
                return Results.Problem("Failed to fetch exchange rates from the API.");
            }

            return Results.Ok(exchangeRates);
        }).WithName("GetLatestExchangeRates");

        // Add endpoint for currency conversion
        app.MapPost("api/exchange-rates/convert",
        async (
            IExchangeRateService exchangeRateService,
            CurrencyConversionRequest currencyConversionRequest
        ) =>
        {
            // Fetch exchange rates
            var exchangeRates = await exchangeRateService.GetConversionRatesAsync(currencyConversionRequest);

            if (exchangeRates == null)
            {
                return Results.Problem("Failed to fetch exchange rates.");
            }
            return Results.Ok(exchangeRates);
        }).WithName("ConvertCurrency");
    }
}
