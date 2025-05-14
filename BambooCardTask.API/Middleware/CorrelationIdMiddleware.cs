namespace BambooCardTask.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, CorrelationIdService correlationIdService)
    {
        context.Response.Headers["X-Correlation-ID"] = correlationIdService.CorrelationId; // Add CorrelationId to response headers
        Log.Information("Adding CorrelationId to response headers: {CorrelationId}", correlationIdService.CorrelationId);

        using (LogContext.PushProperty("CorrelationId", correlationIdService.CorrelationId))
        {
            Log.Information("CorrelationId {CorrelationId} is now part of the log context.", correlationIdService.CorrelationId);
            await _next(context);
        }
    }
}
