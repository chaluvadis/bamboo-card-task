using System.Text.Json;
using BambooCardTask.Interfaces;
using BambooCardTask.Models;
using BambooCardTask.Routes;
using BambooCardTask.Services;

var builder = WebApplication.CreateBuilder(args);

// Bind CurrencyExchange configuration
builder.Services.Configure<CurrencyExchangeConfig>(builder.Configuration.GetSection("CurrencyExchange"));

// Register HttpClient as a service with default base address
builder.Services.AddHttpClient("ExchangeRateClient", client =>
{
    var apiUrl = builder.Configuration["CurrencyExchange:ExchangeRateApiUrl"];
    if (string.IsNullOrEmpty(apiUrl))
    {
        throw new InvalidOperationException("CurrencyExchange:ExchangeRateApiUrl is not configured in appsettings.json");
    }
    client.BaseAddress = new Uri(apiUrl);
});

// Configure JSON serialization options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

// Register ExchangeRateService as a service
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map routes from the Routes folder
app.MapExchangeRateRoutes();

app.Run();
