using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Api.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a success result to problem details.");
        }

        var (status, title) = result.Error.Type switch
        {
            ErrorType.Validation => (StatusCodes.Status400BadRequest, "Validation failed"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        return new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status is StatusCodes.Status500InternalServerError
                ? null
                : result.Error.Name
        })
        {
            StatusCode = status
        };
    }
}
