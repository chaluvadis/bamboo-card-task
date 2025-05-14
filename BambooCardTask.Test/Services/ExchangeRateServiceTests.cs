using System.Net;
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
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
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
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
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

        Assert.Equal("An invalid request URI was provided. Either the request URI must be an absolute URI or BaseAddress must be set.", exception.Message);
    }

    [Fact]
    public async Task GetHistoricalExchangeRatesAsync_ShouldReturnRates_WhenRequestIsValid()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                // The expected structure is {"base":"USD","startDate":"2024-05-01","endDate":"2024-05-02","rates":{"2024-05-01":{"EUR":0.85},"2024-05-02":{"EUR":0.86}}}
                Content = new StringContent("{\"base\":\"USD\",\"startDate\":\"2024-05-01\",\"endDate\":\"2024-05-02\",\"rates\":{\"2024-05-01\":{\"EUR\":0.85},\"2024-05-02\":{\"EUR\":0.86}}}")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.exchangeratesapi.io")
        };
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

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ReturnsNull_OnNonSuccessStatusCode()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
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
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_ReturnsNull_OnDeserializationError()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("not a json")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.exchangeratesapi.io")
        };
        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        // Act & Assert
        var result = await service.GetLatestExchangeRatesAsync("USD");
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLatestExchangeRatesAsync_Handles_EmptyBaseCurrency()
    {
        // Arrange
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var loggerMock = new Mock<ILogger<ExchangeRateService>>();
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"rates\":{\"EUR\":0.85}}")
            });

        var httpClient = new HttpClient(httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.exchangeratesapi.io")
        };
        httpClientFactoryMock.Setup(_ => _.CreateClient("ExchangeRateClient")).Returns(httpClient);

        var service = new ExchangeRateService(httpClientFactoryMock.Object, loggerMock.Object);

        // Act
        var result = await service.GetLatestExchangeRatesAsync(string.Empty);

        // Assert
        Assert.NotNull(result); // The service will call with an empty string, which may result in a bad request or default behavior
    }
}
