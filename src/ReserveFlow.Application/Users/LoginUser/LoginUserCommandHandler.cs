using FluentValidation;
using ReserveFlow.Application.Abstractions.Authentication;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Shared;
using ReserveFlow.Domain.Users;

namespace ReserveFlow.Application.Users.LoginUser;

public sealed class LoginUserCommandHandler(
    IValidator<LoginUserCommand> validator,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenProvider jwtTokenProvider) : ICommandHandler<LoginUserCommand, string>
{
    public async Task<Result<string>> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure<string>(validation.ToError());
        }

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<string>(emailResult.Error);
        }

        var user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return Result.Failure<string>(UserError.InvalidCredentials);
        }

        if (user.Status == UserStatus.Suspended)
        {
            return Result.Failure<string>(UserError.AccountSuspended);
        }

        return jwtTokenProvider.Generate(user.Id, user.Email.Value, user.Roles);
    }
}
