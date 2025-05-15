namespace BambooCardTask.HealthCheck;

public class FrankFurterHealthCheck(IHttpClientFactory httpClientFactory) : IHealthCheck
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.frankfurter.dev/", cancellationToken);
            response.EnsureSuccessStatusCode();
            return HealthCheckResult.Healthy("Frankfurter API is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Frankfurter API is unhealthy", exception: ex);
        }
    }
}