using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BambooCardTask.Test.Routes;

public class ExchangeRateRoutesTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnOk_WhenBaseCurrencyIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnBadRequest_WhenBaseCurrencyIsInvalid()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=INVALID");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Ensure no authentication is configured for this test
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnOk_WhenTokenProvided()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Add mock authentication setup here
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-token");

        // Act
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock authentication to simulate invalid token
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid-token");

        // Act
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnOk_WhenTokenIsValid()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Mock authentication to simulate valid token
            });
        }).CreateClient();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-token");

        // Act
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetConversionRates_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestBody = new
        {
            fromCurrency = "USD",
            targetCurrencies = new[] { "EUR", "GBP" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/exchange-rates/convert", requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetConversionRates_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestBody = new
        {
            fromCurrency = "INVALID",
            targetCurrencies = new[] { "EUR", "GBP" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/exchange-rates/convert", requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetConversionRates_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        // Arrange
        var client = _factory.CreateClient();
        var requestBody = new
        {
            fromCurrency = "USD",
            targetCurrencies = new[] { "EUR", "GBP" }
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/exchange-rates/convert", requestBody);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
