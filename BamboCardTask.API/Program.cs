using Serilog; // Added for structured logging

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration) // Read settings from appsettings.json
        .ReadFrom.Services(services) // Integrate with DI
        .Enrich.FromLogContext() // Add contextual information to logs
        .WriteTo.Console(); // Log to console
});

// aspire support
builder.AddServiceDefaults();

// Bind and register the provider-specific configuration
var providerName = builder.Configuration["Provider:Name"] ?? throw new InvalidOperationException("Provider name is not configured in appsettings.json");

Log.Information("Configuring HttpClient for provider: {ProviderName}", providerName);

var providerSection = builder.Configuration.GetSection(providerName)
                ?? throw new InvalidOperationException($"Configuration section for provider '{providerName}' is not found.");

Log.Information("Provider section: {ProviderSection}", providerSection);

// Bind Provider configuration
var providerConfig = providerSection.Get<ProviderConfig>() ?? throw new InvalidOperationException($"Failed to bind configuration for provider '{providerName}'");
builder.Services.AddSingleton(providerConfig);

// Register HttpClient as a service with default base address and policies
builder.Services.AddHttpClient("ExchangeRateClient", client =>
{
    var baseAddress = providerConfig.ExchangeRateApiUrl;
    Log.Information("Base address for HttpClient: {BaseAddress}", baseAddress);

    if (string.IsNullOrEmpty(baseAddress))
    {
        throw new InvalidOperationException($"ExchangeRateApiUrl is not configured for provider '{providerName}' in appsettings.json");
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

// Request Validation
builder.Services.AddValidation();
builder.Services.AddProblemDetails();

// Response Caching
builder.Services.AddResponseCaching();

// Register ExchangeRateService as a service
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure Serilog is properly disposed on application shutdown
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.MapDefaultEndpoints();

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
