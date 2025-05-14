using System.Threading.Tasks;
using BambooCardTask.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BambooCardTask.Test.Filters
{
    public class ExceptionFilterTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldReturnProblemDetails_ForCustomApiException()
        {
            // Arrange
            var filter = new ExceptionFilter();
            var context = new DefaultHttpContext();
            var invocationContext = new DefaultEndpointFilterInvocationContext(context);
            var next = new EndpointFilterDelegate(async ctx => throw new TestCustomApiException());

            // Act
            var result = await filter.InvokeAsync(invocationContext, next);

            // Assert
            var problemDetails = Assert.IsType<JsonHttpResult<ProblemDetails>>(result);
            Assert.Equal(400, problemDetails?.Value?.Status);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnProblemDetails_ForUnauthorizedAccessException()
        {
            // Arrange
            var filter = new ExceptionFilter();
            var context = new DefaultHttpContext();
            var invocationContext = new DefaultEndpointFilterInvocationContext(context);
            var next = new EndpointFilterDelegate(async ctx => throw new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await filter.InvokeAsync(invocationContext, next);

            // Assert
            var problemDetails = Assert.IsType<JsonHttpResult<ProblemDetails>>(result);
            Assert.Equal(401, problemDetails?.Value?.Status);
        }

        [Fact]
        public async Task InvokeAsync_ShouldReturnProblemDetails_ForGenericException()
        {
            // Arrange
            var filter = new ExceptionFilter();
            var context = new DefaultHttpContext();
            var invocationContext = new DefaultEndpointFilterInvocationContext(context);
            var next = new EndpointFilterDelegate(async ctx => throw new Exception("Internal Server Error"));

            // Act
            var result = await filter.InvokeAsync(invocationContext, next);

            // Assert
            var problemDetails = Assert.IsType<JsonHttpResult<ProblemDetails>>(result);
            Assert.Equal(500, problemDetails?.Value?.Status);
        }

        private class TestCustomApiException : CustomApiExceptionBase
        {
            public override int StatusCode => 400;
        }
    }
}
