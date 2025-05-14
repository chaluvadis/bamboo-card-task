namespace BambooCardTask.Configuration;

public static class HttpClientConfiguration
{
    public static void ConfigureHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var providerName = configuration["Provider:Name"]
            ?? throw new InvalidOperationException("Provider name is not configured in appsettings.json");
        Log.Information("Starting HttpClient configuration for provider: {ProviderName}", providerName);

        var providerSection = configuration.GetSection(providerName)
            ?? throw new InvalidOperationException($"Configuration section for provider '{providerName}' is not found.");
        Log.Information("Provider section retrieved: {ProviderSection}", providerSection);

        var providerConfig = providerSection.Get<ProviderConfig>()
            ?? throw new InvalidOperationException($"Failed to bind configuration for provider '{providerName}'");
        Log.Information("Provider configuration bound successfully for provider: {ProviderName}", providerName);

        services.AddSingleton(providerConfig);

        services.AddHttpClient("ExchangeRateClient", client =>
        {
            var baseAddress = providerConfig.ExchangeRateApiUrl;
            if (string.IsNullOrEmpty(baseAddress))
            {
                throw new InvalidOperationException($"ExchangeRateApiUrl is not configured for provider '{providerName}' in appsettings.json");
            }
            Log.Information("Setting base address for HttpClient: {BaseAddress}", baseAddress);
            client.BaseAddress = new Uri(baseAddress);
        })
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
           retryCount: 3,
           retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        ))
        .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)));

        Log.Information("HttpClient configuration completed successfully for provider: {ProviderName}", providerName);
    }
}
