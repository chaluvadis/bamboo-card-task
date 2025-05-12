using BambooCardTask.Models;
namespace BambooCardTask.Interfaces;
public interface IExchangeRateService
{
    ValueTask<ExchangeRates?> GetLatestExchangeRatesAsync(string baseCurrency);
    ValueTask<ConversionRates?> GetConversionRatesAsync(CurrencyConversionRequest currencyConversionRequest);
}