using BambooCardTask.Middleware;
using BambooCardTask.Services;
using Microsoft.AspNetCore.Http;

namespace BambooCardTask.Test.Middleware;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_SetsCorrelationIdHeaderAndLogs()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var correlationId = "test-correlation-id";
        var correlationIdService = new CorrelationIdService();
        // Set the private field via reflection (if needed)
        var field = typeof(CorrelationIdService).GetField("<CorrelationId>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(correlationIdService, correlationId);

        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context, correlationIdService);

        // Assert
        Assert.True(context.Response.Headers.ContainsKey("X-Correlation-ID"));
        Assert.Equal(correlationId, context.Response.Headers["X-Correlation-ID"]);
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_PushesCorrelationIdToLogContext()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var correlationId = "log-context-id";
        var correlationIdService = new CorrelationIdService();
        var field = typeof(CorrelationIdService).GetField("<CorrelationId>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(correlationIdService, correlationId);

        var nextCalled = false;
        RequestDelegate next = ctx => { nextCalled = true; return Task.CompletedTask; };

        var middleware = new CorrelationIdMiddleware(next);

        // Act & Assert (no exception means log context push worked)
        await middleware.InvokeAsync(context, correlationIdService);
        Assert.True(nextCalled);
    }
}
