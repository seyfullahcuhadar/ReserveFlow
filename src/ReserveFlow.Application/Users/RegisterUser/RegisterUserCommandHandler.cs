using FluentValidation;
using ReserveFlow.Application.Abstractions.Authentication;
using ReserveFlow.Application.Diagnostics;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Shared;
using ReserveFlow.Domain.Users;

namespace ReserveFlow.Application.Users.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IValidator<RegisterUserCommand> _validator;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly TimeProvider _timeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IValidator<RegisterUserCommand> validator,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        TimeProvider timeProvider,
        IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _timeProvider = timeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        using var activity = ApplicationDiagnostics.ActivitySource.StartActivity("RegisterUser");

        var validation = await _validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure<Guid>(validation.ToError());
        }

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            return Result.Failure<Guid>(emailResult.Error);
        }

        var email = emailResult.Value;

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            return Result.Failure<Guid>(UserError.EmailAlreadyRegistered);
        }

        var passwordHash = _passwordHasher.Hash(command.Password);
        var userResult = User.Register(email, passwordHash, _timeProvider.GetUtcNow().UtcDateTime);
        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;

        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        activity?.SetTag("reserveflow.user.id", user.Id);
        ApplicationDiagnostics.UsersRegistered.Add(1);

        return user.Id;
    }
}
