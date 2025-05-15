using System.ComponentModel.DataAnnotations;
using BambooCardTask.Models;

namespace BambooCardTask.Test.Models;

public class ModelValidationsTests
{
    [Fact]
    public void ValidCurrencyListAttribute_ShouldReturnError_WhenListIsEmpty()
    {
        // Arrange
        var attribute = new ValidCurrencyListAttribute();
        var validationContext = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(new List<string>(), validationContext);

        // Assert
        Assert.Equal("Please provide at least one target currency.", result?.ErrorMessage);
    }

    [Fact]
    public void ValidCurrencyListAttribute_ShouldReturnError_WhenListContainsExcludedCurrencies()
    {
        // Arrange
        var attribute = new ValidCurrencyListAttribute();
        var validationContext = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(new List<string> { "TRY", "USD" }, validationContext);

        // Assert
        Assert.Equal("Currencies TRY are not allowed to process.", result?.ErrorMessage);
    }

    [Fact]
    public void ValidCurrencyListAttribute_ShouldPass_WhenListIsValid()
    {
        // Arrange
        var attribute = new ValidCurrencyListAttribute();
        var validationContext = new ValidationContext(new object());

        // Act
        var result = attribute.GetValidationResult(new List<string> { "USD", "EUR" }, validationContext);

        // Assert
        Assert.Null(result);
    }
}
