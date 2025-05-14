using BambooCardTask.Models;
using BambooCardTask.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BambooCardTask.Test.Services;

public class ExchangeRateServiceTests
{
    [Fact]
    public async Task GetLatestExchangeRatesAsync_ShouldReturnRates_WhenBaseCurrencyIsValid()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpClient = new HttpClient();
        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetLatestExchangeRatesAsync("USD");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetConversionRatesAsync_ShouldReturnRates_WhenRequestIsValid()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpClient = new HttpClient();
        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        var request = new CurrencyConversionRequest
        {
            FromCurrency = "USD",
            TargetCurrencies = ["EUR", "GBP"]
        };

        // Act
        var result = await service.GetConversionRatesAsync(request);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ShouldReturnRates_WhenRequestIsValid()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpClient = new HttpClient();
        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        var request = new HistoricalExchangeRatesRequest
        {
            StartDate = DateTime.UtcNow.AddDays(-7),
            EndDate = DateTime.UtcNow,
            FromCurrency = "USD"
        };

        // Act
        var result = await service.GetHistoricalExchangeRatesAsync(request);

        // Assert
        Assert.NotNull(result);
    }
}
