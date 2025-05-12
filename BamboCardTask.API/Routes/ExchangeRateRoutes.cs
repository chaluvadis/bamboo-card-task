using BambooCardTask.Interfaces;

namespace BambooCardTask.Routes;

public static class ExchangeRateRoutes
{
    public static void MapExchangeRateRoutes(this WebApplication app)
    {
        // Add endpoint to retrieve latest exchange rates
        app.MapGet("api/exchange-rates/latest",
        async (
            IExchangeRateService exchangeRateService,
            string baseCurrency
        ) =>
        {
            // Fetch data from the service
            var exchangeRates = await exchangeRateService.GetLatestExchangeRatesAsync(baseCurrency);

            if (exchangeRates == null)
            {
                return Results.Problem("Failed to fetch exchange rates from the API.");
            }

            return Results.Ok(exchangeRates);
        }).WithName("GetLatestExchangeRates");
    }
}
