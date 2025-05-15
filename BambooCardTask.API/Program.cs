
var builder = WebApplication.CreateBuilder(args);


// Add health checks: custom and FrankFurter API
builder.Services.AddHealthChecks()
    .AddCheck<FrankFurterHealthCheck>(
        "frankfurter_api",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready", "live"]);

// Configure Serilog for structured logging
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration) // Read settings from appsettings.json
        .ReadFrom.Services(services) // Integrate with DI
        .Enrich.FromLogContext() // Add contextual information to logs
        .WriteTo.Console(); // Log to console
});

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
builder.Services.AddScoped<CorrelationIdService>();
builder.Services.AddOpenApi();

builder.Services.ConfigureJwtAuthentication(builder.Configuration);

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443; // Default HTTPS port
});


var app = builder.Build();
app.MapHealthChecks("api/health");
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRateLimiter();
app.UseResponseCaching();
app.UseMiddleware<CacheControlMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();


// Only use HTTPS redirection if not running in test environment
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<CorrelationIdMiddleware>();


// Add authentication and authorization before mapping endpoints
app.UseAuthentication();
app.UseAuthorization();

app.MapExchangeRateRoutes();


// Ensure Serilog is properly disposed on application shutdown
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();