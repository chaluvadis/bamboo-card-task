using System.Text.Json;
using System.Threading.RateLimiting;
using BambooCardTask.Interfaces;
using BambooCardTask.Models;
using BambooCardTask.Routes;
using BambooCardTask.Services;
using Microsoft.Net.Http.Headers;
using Polly;
var builder = WebApplication.CreateBuilder(args);

// Bind CurrencyExchange configuration
builder.Services.Configure<CurrencyExchangeConfig>(builder.Configuration.GetSection("CurrencyExchange"));

// Register HttpClient as a service with default base address and policies
builder.Services.AddHttpClient("ExchangeRateClient", client =>
{
    var baseAddress = builder.Configuration["CurrencyExchange:ExchangeRateApiUrl"];
    if (string.IsNullOrEmpty(baseAddress))
    {
        throw new InvalidOperationException("CurrencyExchange:ExchangeRateApiUrl is not configured in appsettings.json");
    }
    client.BaseAddress = new Uri(baseAddress);
})
.AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30)));

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// Configure JSON serialization options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddValidation();
builder.Services.AddProblemDetails();

builder.Services.AddResponseCaching();

// Register ExchangeRateService as a service
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRateLimiter();
app.UseResponseCaching();

app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(60)
    };

    // Add Vary header for the User-Agent
    context.Response.Headers[HeaderNames.Vary] = "User-Agent";

    await next();
});

app.UseHttpsRedirection();

// Map routes from the Routes folder
app.MapExchangeRateRoutes();

app.Run();
