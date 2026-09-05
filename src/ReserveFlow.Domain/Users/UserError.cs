using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Users;

public static class UserError
{
    public static readonly Error PasswordHashRequired = new(
        "Users.PasswordHashRequired",
        "Password hash is required.",
        ErrorType.Validation);

    public static readonly Error EmailAlreadyRegistered = new(
        "Users.EmailAlreadyRegistered",
        "Email is already registered.",
        ErrorType.Conflict);

    public static readonly Error InvalidCredentials = new(
        "Users.InvalidCredentials",
        "Invalid credentials.",
        ErrorType.Unauthorized);

    public static readonly Error AccountSuspended = new(
        "Users.AccountSuspended",
        "Account is suspended.",
        ErrorType.Unauthorized);
}
