namespace BambooCardTask.Middleware;

public class CacheControlMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        Log.Information("Setting Cache-Control headers for the response.");

        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(60)
        };

        Log.Information("Added Vary header for User-Agent.");

        context.Response.Headers[HeaderNames.Vary] = "User-Agent";

        await _next(context);
    }
}
