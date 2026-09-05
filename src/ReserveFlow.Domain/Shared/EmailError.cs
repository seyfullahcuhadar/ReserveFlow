using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Shared;

public static class EmailError
{
    public static readonly Error Required = new(
        "Shared.EmailRequired",
        "Email is required.",
        ErrorType.Validation);

    public static readonly Error InvalidFormat = new(
        "Shared.EmailInvalidFormat",
        "Email format is invalid.",
        ErrorType.Validation);
}
