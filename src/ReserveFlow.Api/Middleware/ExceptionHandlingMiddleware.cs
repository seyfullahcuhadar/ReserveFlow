using Microsoft.AspNetCore.Mvc;

namespace ReserveFlow.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (!context.RequestAborted.IsCancellationRequested)
        {
            logger.LogError(exception, "Unhandled exception while processing the request.");
            await WriteProblemAsync(context);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context)
    {
        const int statusCode = StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = "An unexpected error occurred"
        });
    }
}
