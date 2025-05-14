namespace BambooCardTask.Filters;
public abstract class CustomApiExceptionBase : Exception
{
    public abstract int StatusCode { get; }

    public virtual ProblemDetails GetProblemDetails()
    {
        return new ProblemDetails
        {
            Title = "Error",
            Detail = $"{GetType().Name}: {Message}",
            Status = StatusCode,
        };
    }
}
public class ExceptionFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (CustomApiExceptionBase exception)
        {
            var problemDetails = exception.GetProblemDetails();
            var statusCode = exception.StatusCode;
            return Results.Json(problemDetails, contentType: "application/problem+json", statusCode: statusCode);
        }
        catch (UnauthorizedAccessException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            };
            return Results.Json(problemDetails, contentType: "application/problem+json", statusCode: StatusCodes.Status401Unauthorized);
        }
        catch (Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            };
            return Results.Json(problemDetails, contentType: "application/problem+json", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
