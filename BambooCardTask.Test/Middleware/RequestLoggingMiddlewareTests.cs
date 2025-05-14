using System.Threading.Tasks;
using BambooCardTask.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Serilog;
using Xunit;

namespace BambooCardTask.Test.Middleware
{
    public class RequestLoggingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_SetsCorrelationIdHeader_IfNotAlreadySet()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var nextCalled = false;
            RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
            var middleware = new RequestLoggingMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(context.Response.Headers.ContainsKey("X-Correlation-ID"));
            Assert.False(string.IsNullOrEmpty(context.Response.Headers["X-Correlation-ID"]));
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task InvokeAsync_DoesNotOverwriteCorrelationIdHeader_IfAlreadySet()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var existingId = "existing-correlation-id";
            context.Response.Headers["X-Correlation-ID"] = existingId;
            var nextCalled = false;
            RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };
            var middleware = new RequestLoggingMiddleware(next);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal(existingId, context.Response.Headers["X-Correlation-ID"]);
            Assert.True(nextCalled);
        }
    }
}
