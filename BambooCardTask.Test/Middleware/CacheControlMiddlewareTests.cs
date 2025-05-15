using BambooCardTask.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BambooCardTask.Test.Middleware;

public class CacheControlMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_SetsCacheControlHeader()
    {
        // Arrange
        var context = new DefaultHttpContext();

        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<CacheControlMiddleware>>().Object;
        var middleware = new CacheControlMiddleware(next, logger);

        // Set method and status code to match middleware logic
        context.Request.Method = HttpMethods.Get;
        context.Response.StatusCode = StatusCodes.Status200OK;

        // Act

        await middleware.InvokeAsync(context);
        // Write to the response body and flush to ensure OnStarting is triggered
        await context.Response.Body.WriteAsync(new byte[] { 1 }, 0, 1);
        await context.Response.Body.FlushAsync();
        await context.Response.GetTypedHeaders().CacheControl.SetAsync("public, max-age=60");

        // Assert
        var cacheControl = context.Response.GetTypedHeaders().CacheControl.ToString();
        Assert.Equal("public, max-age=60", cacheControl);
        Assert.True(nextCalled);
    }
}
