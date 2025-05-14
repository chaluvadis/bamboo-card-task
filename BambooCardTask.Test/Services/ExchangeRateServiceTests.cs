using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BambooCardTask.Models;
using BambooCardTask.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace BambooCardTask.Test.Services;

public class ExchangeRateServiceTests
{
    [Fact]
    public async Task GetLatestExchangeRatesAsync_ShouldReturnRates_WhenBaseCurrencyIsValid()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"rates\":{\"EUR\":0.85,\"GBP\":0.75}}")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.exchangeratesapi.io")
        };

        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetLatestExchangeRatesAsync("USD");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.85, result.Rates["EUR"]);
        Assert.Equal(0.75, result.Rates["GBP"]);
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
    public async Task GetConversionRatesAsync_ShouldThrowException_WhenBaseAddressIsMissing()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpClient = new HttpClient(); // No BaseAddress set

        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.GetConversionRatesAsync(new CurrencyConversionRequest { FromCurrency = "USD", TargetCurrencies = ["EUR"] }));

        Assert.Equal("BaseAddress must be set for the HttpClient.", exception.Message);
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
