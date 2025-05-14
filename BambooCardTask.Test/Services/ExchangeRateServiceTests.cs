using BambooCardTask.Models;
using BambooCardTask.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BambooCardTask.Test.Services;

public class ExchangeRateServiceTests
{
    private Mock<IHttpClientFactory> CreateHttpClientFactoryMock()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var httpClient = new HttpClient();
        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);
        return httpClientFactoryMock;
    }

    [Fact]
    public async Task GetConversionRatesAsync_ShouldReturnRates_WhenValidRequest()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpClientFactoryMock = CreateHttpClientFactoryMock();

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
}
