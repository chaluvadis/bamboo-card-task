using BambooCardTask.Models;
namespace BambooCardTask.Interfaces;
public interface IExchangeRateService
{
    ValueTask<ExchangeRates?> GetLatestExchangeRatesAsync(string baseCurrency);
}