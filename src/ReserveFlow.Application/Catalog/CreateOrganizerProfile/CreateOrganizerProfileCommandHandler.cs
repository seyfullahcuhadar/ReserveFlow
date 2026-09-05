using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Users;

namespace ReserveFlow.Application.Catalog.CreateOrganizerProfile;

public sealed class CreateOrganizerProfileCommandHandler(
    IValidator<CreateOrganizerProfileCommand> validator,
    IUserRepository userRepository,
    IOrganizerProfileRepository organizerProfileRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOrganizerProfileCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateOrganizerProfileCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure<Guid>(validation.ToError());
        }

        if (!await userRepository.ExistsByIdAsync(command.UserId, cancellationToken))
        {
            return Result.Failure<Guid>(CatalogError.UserNotFound);
        }

        if (await organizerProfileRepository.ExistsByUserIdAsync(command.UserId, cancellationToken))
        {
            return Result.Failure<Guid>(CatalogError.OrganizerProfileAlreadyExists);
        }

        var profile = OrganizerProfile.Create(
            command.UserId,
            command.DisplayName,
            command.Bio,
            timeProvider.GetUtcNow().UtcDateTime);

        organizerProfileRepository.Add(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return profile.Id;
    }
}
