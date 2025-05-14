
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

// Bind and register the provider-specific configuration
var providerName = builder.Configuration["Provider:Name"] ?? throw new InvalidOperationException("Provider name is not configured in appsettings.json");
Log.Information("Configuring HttpClient for provider: {ProviderName}", providerName);
var providerSection = builder.Configuration.GetSection(providerName)
                ?? throw new InvalidOperationException($"Configuration section for provider '{providerName}' is not found.");
Log.Information("Provider section: {ProviderSection}", providerSection);
var providerConfig = providerSection.Get<ProviderConfig>() ?? throw new InvalidOperationException($"Failed to bind configuration for provider '{providerName}'");

builder.Services.AddSingleton(providerConfig);

// Configure HttpClient
builder.Services.ConfigureHttpClient(builder.Configuration);
// Add additional services
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

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddResponseCaching();
builder.Services.AddSingleton<IExchangeRateService, ExchangeRateService>();
builder.Services.AddOpenApi();
builder.Services.AddTransient<ICorrelationIdService, CorrelationIdService>();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443; // Default HTTPS port
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRateLimiter();
app.UseResponseCaching();
app.UseMiddleware<CacheControlMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.MapExchangeRateRoutes();

// Ensure Serilog is properly disposed on application shutdown
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();


