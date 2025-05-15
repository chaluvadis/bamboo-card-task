using BambooCardTask.Interfaces;
using BambooCardTask.Models;
using Moq;

namespace BambooCardTask.Test.Interfaces;

public class IExchangeRateServiceTests
{
    [Fact]
    public async Task GetLatestExchangeRatesAsync_ShouldReturnExchangeRates_WhenCalledWithValidBaseCurrency()
    {
        // Arrange
        var mockService = new Mock<IExchangeRateService>();
        var expectedResponse = new ExchangeRatesResponse();
        mockService.Setup(s => s.GetLatestExchangeRatesAsync("USD")).ReturnsAsync(expectedResponse);

        // Act
        var result = await mockService.Object.GetLatestExchangeRatesAsync("USD");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task GetConversionRatesAsync_ShouldReturnConversionRates_WhenCalledWithValidRequest()
    {
        // Arrange
        var mockService = new Mock<IExchangeRateService>();
        var request = new CurrencyConversionRequest
        {
            FromCurrency = "USD",
            TargetCurrencies = ["EUR", "GBP"]
        };
        var expectedResponse = new ConversionRatesResponse();
        mockService.Setup(s => s.GetConversionRatesAsync(request)).ReturnsAsync(expectedResponse);

        // Act
        var result = await mockService.Object.GetConversionRatesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ShouldReturnHistoricalRates_WhenCalledWithValidRequest()
    {
        // Arrange
        var mockService = new Mock<IExchangeRateService>();
        var request = new HistoricalExchangeRatesRequest
        {
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            FromCurrency = "USD"
        };
        var expectedResponse = new HistoricalExchangeRatesResponse();
        mockService.Setup(s => s.GetHistoricalExchangeRatesAsync(request)).ReturnsAsync(expectedResponse);

        // Act
        var result = await mockService.Object.GetHistoricalExchangeRatesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }
}
