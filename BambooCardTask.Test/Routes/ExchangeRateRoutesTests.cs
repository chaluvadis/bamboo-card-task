using System.Net;
using System.Net.Http.Json;
using BambooCardTask.Test.AuthHandler;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace BambooCardTask.Test.Routes;

public class ExchangeRateRoutesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public ExchangeRateRoutesTests(WebApplicationFactory<Program> factory)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("environment", "Test");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "b8e2f3c4d5a6e7f8g9h0i1j2k3l4m5n6o7p8q9r0s1t2u3v4w5x6y7z8a9b0c1d2",
                    ["Jwt:Issuer"] = "bamboocard.ae",
                    ["Jwt:Audience"] = "bamboocard.ae"
                };
                config.AddInMemoryCollection(dict);
            });
            builder.ConfigureServices(services =>
            {
                // Remove HTTPS redirection for tests
                var descriptor = services.FirstOrDefault(d => d.ServiceType.FullName == "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions");
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                // Ensure CorrelationIdService is registered for middleware
                services.AddScoped<BambooCardTask.Services.CorrelationIdService>();

                // Add both JWT and test authentication schemes
                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "CombinedScheme";
                })
                .AddPolicyScheme("CombinedScheme", "Combined Auth", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        // Use test scheme if header is present, else JWT
                        if (context.Request.Headers.TryGetValue("X-Test-Auth", out var value) && value == "true")
                            return TestAuthHandler.TestScheme;
                        return "JwtBearer";
                    };
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.TestScheme, _ => { })
                .AddJwtBearer("JwtBearer", options =>
                {
                    var sp = services.BuildServiceProvider();
                    var config = sp.GetRequiredService<IConfiguration>();
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                    };
                });
            });
        });
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnOk_WhenBaseCurrencyIsValid()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnBadRequest_WhenBaseCurrencyIsInvalid()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=INVALID");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "invalid-token");
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLatestExchangeRates_ShouldReturnOk_WhenTokenIsValid()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
        var response = await client.GetAsync("/api/exchange-rates/latest?baseCurrency=USD");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetConversionRates_ShouldReturnOk_WhenRequestIsValid()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
        var requestBody = new
        {
            fromCurrency = "USD",
            targetCurrencies = new[] { "EUR", "GBP" }
        };
        var response = await client.PostAsJsonAsync("/api/exchange-rates/convert", requestBody);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetConversionRates_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
        var requestBody = new
        {
            fromCurrency = "INVALID",
            targetCurrencies = new[] { "EUR", "GBP" }
        };
        var response = await client.PostAsJsonAsync("/api/exchange-rates/convert", requestBody);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetConversionRates_ShouldReturnUnauthorized_WhenNoTokenProvided()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var requestBody = new
        {
            fromCurrency = "USD",
            targetCurrencies = new[] { "EUR", "GBP" }
        };
        var response = await client.PostAsJsonAsync("/api/exchange-rates/convert", requestBody);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}