namespace BambooCardTask.Models;

public record ExchangeRates(
    float Amount,
    string Base,
    DateOnly Date,
    Dictionary<string, double> Rates
);

public record CurrencyExchangeConfig(
    string BaseCurrency,
    string ExchangeRateApiUrl
);