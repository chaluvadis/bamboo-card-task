namespace BambooCardTask.Models;

public class CurrencyConversionRequest
{
    [Required]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "FromCurrency must be a 3-letter uppercase string.")]
    public string FromCurrency { get; set; } = string.Empty;

    [Required]
    [ValidCurrencyList]
    public List<string> TargetCurrencies { get; set; } = [];
}

public class ExchangeRatesResponse
{
    public float Amount { get; set; }
    public string Base { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public Dictionary<string, double> Rates { get; set; } = [];
}


public class ConversionRatesResponse
{
    public float Amount { get; set; }
    public string Base { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public Dictionary<string, double> Rates { get; set; } = [];
}

public class HistoricalExchangeRatesRequest
{
    [Required]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "FromCurrency must be a 3-letter uppercase string.")]
    public string FromCurrency { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}

public class HistoricalExchangeRatesResponse
{
    public string Base { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Dictionary<DateOnly, Dictionary<string, double>> Rates { get; set; } = new();
}

public class ProviderConfig
{
    public string Name { get; set; } = string.Empty;
    public string BaseCurrency { get; set; } = string.Empty;
    public string ExchangeRateApiUrl { get; set; } = string.Empty;
    public List<string> ExcludedCurrencies { get; set; } = [];
}