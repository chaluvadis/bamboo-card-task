
namespace BambooCardTask.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var clientId = context.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "Unknown";
        var httpMethod = context.Request.Method;
        var targetEndpoint = context.Request.Path;
        var responseCode = context.Response.StatusCode;
        var responseTime = stopwatch.ElapsedMilliseconds;

        Log.Information("Request Details: Client IP: {ClientIp}, ClientId: {ClientId}, HTTP Method: {HttpMethod}, Target Endpoint: {TargetEndpoint}, Response Code: {ResponseCode}, Response Time: {ResponseTime}ms",
            clientIp, clientId, httpMethod, targetEndpoint, responseCode, responseTime);
    }
}
