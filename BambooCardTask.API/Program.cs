var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogLogging();
builder.Services.AddHealthChecks()
    .AddCheck<FrankFurterHealthCheck>(
        "frankfurter_api",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready", "live"]);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddResponseCaching();
builder.Services.AddSingleton<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<CorrelationIdService>();
builder.Services.AddOpenApi();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443; // Default HTTPS port
});

builder.Services.ConfigureHttpClient(builder.Configuration);
builder.Services.AddCustomRateLimiter();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddResponseCaching();
builder.Services.AddSingleton<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<CorrelationIdService>();
builder.Services.AddOpenApi();

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
});

// --- App Pipeline ---
var app = builder.Build();

app.MapHealthChecks("api/health");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRateLimiter();
app.UseResponseCaching();
app.UseMiddleware<CacheControlMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapExchangeRateRoutes();

// Ensure Serilog is properly disposed on application shutdown
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();