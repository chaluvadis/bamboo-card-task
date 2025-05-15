namespace BambooCardTask.HealthCheck;

public class FrankFurterHealthCheck(IHttpClientFactory httpClientFactory) : IHealthCheck
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<HealthCheckResponse>("https://api.frankfurter.dev/", cancellationToken);
            if (response == null)
            {
                return HealthCheckResult.Unhealthy("Frankfurter API is unhealthy");
            }
            return HealthCheckResult.Healthy("Frankfurter API is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Frankfurter API is unhealthy", exception: ex);
        }
    }
}