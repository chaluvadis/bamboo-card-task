namespace BambooCardTask.Middleware;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, CorrelationIdService correlationIdService)
    {
        context.Response.Headers["X-Correlation-ID"] = correlationIdService.CorrelationId; // Add CorrelationId to response headers

        using (LogContext.PushProperty("CorrelationId", correlationIdService.CorrelationId))
        {
            await _next(context);
        }
    }
}
