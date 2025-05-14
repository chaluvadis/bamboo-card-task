namespace BambooCardTask.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        Log.Information("Request started: {HttpMethod} {TargetEndpoint}", context.Request.Method, context.Request.Path);

        // Set correlation ID header before response starts (if not already set by other middleware)
        if (!context.Response.HasStarted && !context.Response.Headers.ContainsKey("X-Correlation-ID"))
        {
            var correlationId = context.Items.ContainsKey("CorrelationId")
                ? context.Items["CorrelationId"]?.ToString()
                : Guid.NewGuid().ToString("N");
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            context.Items["CorrelationId"] = correlationId;
        }

        Log.Information("Request completed: {HttpMethod} {TargetEndpoint} with status code {ResponseCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);

        stopwatch.Stop();

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var clientId = context.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "Unknown";
        var httpMethod = context.Request.Method;
        var targetEndpoint = context.Request.Path;
        var responseCode = context.Response.StatusCode;
        var responseTime = stopwatch.ElapsedMilliseconds;

        var correlationIdLog = context.Items.ContainsKey("CorrelationId") ? context.Items["CorrelationId"]?.ToString() : "N/A";

        Log.Information("Correlation ID: {CorrelationId} - Request Details: Client IP: {ClientIp}, ClientId: {ClientId}, HTTP Method: {HttpMethod}, Target Endpoint: {TargetEndpoint}, Response Code: {ResponseCode}, Response Time: {ResponseTime}ms",
            correlationIdLog, clientIp, clientId, httpMethod, targetEndpoint, responseCode, responseTime);

        await _next(context);
    }
}
