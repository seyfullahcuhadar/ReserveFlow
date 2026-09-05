using FluentValidation.Results;
using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Application.Validation;

internal static class ValidationErrors
{
    public static Error ToError(this ValidationResult validation) =>
        Error.Validation(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
}
