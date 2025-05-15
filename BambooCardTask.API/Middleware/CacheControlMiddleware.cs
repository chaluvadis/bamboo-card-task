namespace BambooCardTask.Middleware;

public class CacheControlMiddleware(RequestDelegate next, ILogger<CacheControlMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<CacheControlMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Only cache GET requests
        if (context.Request.Method == HttpMethods.Get)
        {
            // Set headers before the response starts
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == StatusCodes.Status200OK)
                {
                    _logger.LogInformation("Setting Cache-Control headers for the response.");
                    context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(60)
                    };
                    _logger.LogInformation("Added Vary header for User-Agent.");
                    context.Response.Headers[HeaderNames.Vary] = "User-Agent";
                }
                return Task.CompletedTask;
            });
            await _next(context);
        }
        else
        {
            await _next(context);
        }
    }
}
