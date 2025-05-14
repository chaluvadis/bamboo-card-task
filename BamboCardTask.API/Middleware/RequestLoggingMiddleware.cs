namespace BambooCardTask.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        Log.Information("Request started: {HttpMethod} {TargetEndpoint}", context.Request.Method, context.Request.Path);

        await _next(context);

        Log.Information("Request completed: {HttpMethod} {TargetEndpoint} with status code {ResponseCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);

        stopwatch.Stop();

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var clientId = context.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "Unknown";
        var httpMethod = context.Request.Method;
        var targetEndpoint = context.Request.Path;
        var responseCode = context.Response.StatusCode;
        var responseTime = stopwatch.ElapsedMilliseconds;

        var correlationId = Guid.NewGuid().ToString("N"); // Use the new .NET 10 format for GUIDs
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId; // Add CorrelationId to the response headers

        Log.Information("Correlation ID: {CorrelationId} - Request Details: Client IP: {ClientIp}, ClientId: {ClientId}, HTTP Method: {HttpMethod}, Target Endpoint: {TargetEndpoint}, Response Code: {ResponseCode}, Response Time: {ResponseTime}ms",
            correlationId, clientIp, clientId, httpMethod, targetEndpoint, responseCode, responseTime);
    }
}
