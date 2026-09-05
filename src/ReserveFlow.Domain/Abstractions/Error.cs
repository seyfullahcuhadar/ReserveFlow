namespace ReserveFlow.Domain.Abstractions;

public record Error(string Code, string Name, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");

    public static Error Validation(string name) =>
        new("General.Validation", name, ErrorType.Validation);
}
