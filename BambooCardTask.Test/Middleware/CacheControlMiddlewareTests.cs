using System.Threading.Tasks;
using BambooCardTask.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace BambooCardTask.Test.Middleware
{
    public class CacheControlMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_SetsCacheControlHeader()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var nextCalled = false;
            RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
            var middleware = new CacheControlMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(context.Response.Headers.ContainsKey("Cache-Control"));
            var cacheControl = context.Response.Headers["Cache-Control"].ToString();
            Assert.Equal("public, max-age=60", cacheControl);
            Assert.True(nextCalled);
        }
    }
}
