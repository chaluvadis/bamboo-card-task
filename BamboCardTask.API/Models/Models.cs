using System.ComponentModel.DataAnnotations;

namespace BambooCardTask.Models;

public class ValidCurrencyListAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not List<string> currencies)
        {
            return new ValidationResult("Invalid currency list.");
        }

        if (currencies.Count == 0)
        {
            return new ValidationResult("Please provide at least one target currency.");
        }

        var configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration))!;
        var excludedCurrencies = configuration.GetSection("CurrencyExchange:ExcludedCurrencies").Get<List<string>>()
            ?? ["TRY", "PLN", "THB", "MXN"];

        var invalidCurrencies = currencies.Where(currency => excludedCurrencies.Contains(currency)).ToList();

        if (invalidCurrencies.Any())
        {
            return new ValidationResult($"Currencies {string.Join(", ", invalidCurrencies)} are not allowed process");
        }

        return ValidationResult.Success;
    }
}



public record CurrencyExchangeConfig(
    string BaseCurrency,
    string ExchangeRateApiUrl
);

public class CurrencyConversionRequest
{
    [Required]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "FromCurrency must be a 3-letter uppercase string.")]
    public string FromCurrency { get; set; } = string.Empty;

    [Required]
    [ValidCurrencyList]
    public List<string> TargetCurrencies { get; set; } = [];
}

public record ExchangeRates(
    float Amount,
    string Base,
    DateOnly Date,
    Dictionary<string, double> Rates
);

public record ConversionRates
{
    public float Amount { get; set; }
    public string Base { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public Dictionary<string, double> Rates { get; set; } = [];
}