namespace BambooCardTask.Interfaces;
public interface IExchangeRateService
{
    ValueTask<ExchangeRatesResponse?> GetLatestExchangeRatesAsync(string baseCurrency);
    ValueTask<ConversionRatesResponse?> GetConversionRatesAsync(CurrencyConversionRequest currencyConversionRequest);
    ValueTask<HistoricalExchangeRatesResponse?> GetHistoricalExchangeRatesAsync(HistoricalExchangeRatesRequest historicalExchangeRates);
}