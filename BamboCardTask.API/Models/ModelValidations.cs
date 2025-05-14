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

// public class ValidStartDateAttribute : ValidationAttribute
// {
//     protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
//     {
//         if (value is not DateOnly startDate)
//         {
//             return new ValidationResult("Invalid start date.");
//         }

//         if (startDate > DateOnly.FromDateTime(DateTime.Now))
//         {
//             return new ValidationResult("Start date cannot be a future date.");
//         }

//         return ValidationResult.Success;
//     }
// }

// public class ValidEndDateAttribute : ValidationAttribute
// {
//     protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
//     {
//         if (value is not DateOnly endDate)
//         {
//             return new ValidationResult("Invalid end date.");
//         }

//         var instance = validationContext.ObjectInstance as HistoricalExchangeRatesRequest;
//         if (instance == null)
//         {
//             return new ValidationResult("Invalid object instance.");
//         }

//         if (endDate < instance.StartDate)
//         {
//             return new ValidationResult("End date cannot be earlier than start date.");
//         }

//         if (endDate > DateOnly.FromDateTime(DateTime.Now))
//         {
//             return new ValidationResult("End date cannot be a future date.");
//         }

//         return ValidationResult.Success;
//     }
// }
