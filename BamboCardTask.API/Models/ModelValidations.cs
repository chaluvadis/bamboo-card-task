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

        var excludedCurrencies = new List<string> { "TRY", "PLN", "THB", "MXN" };

        var invalidCurrencies = currencies.Where(currency => excludedCurrencies.Contains(currency)).ToList();

        if (invalidCurrencies.Any())
        {
            return new ValidationResult($"Currencies {string.Join(", ", invalidCurrencies)} are not allowed to process.");
        }

        return ValidationResult.Success;
    }
}
