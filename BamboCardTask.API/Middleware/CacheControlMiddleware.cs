namespace BambooCardTask.Middleware;

public class CacheControlMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(60)
        };

        // Add Vary header for the User-Agent
        context.Response.Headers[HeaderNames.Vary] = "User-Agent";

        await _next(context);
    }
}
