using FluentValidation;
using ReserveFlow.Application.Exceptions;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Users;
using ValidationException = ReserveFlow.Application.Exceptions.ValidationException;

namespace ReserveFlow.Application.Catalog.CreateOrganizerProfile;

public sealed class CreateOrganizerProfileCommandHandler(
    IValidator<CreateOrganizerProfileCommand> validator,
    IUserRepository userRepository,
    IOrganizerProfileRepository organizerProfileRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOrganizerProfileCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateOrganizerProfileCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (!await userRepository.ExistsByIdAsync(command.UserId, cancellationToken))
        {
            throw new ValidationException("User was not found.");
        }

        if (await organizerProfileRepository.ExistsByUserIdAsync(command.UserId, cancellationToken))
        {
            throw new ConflictException("Organizer profile already exists for this user.");
        }

        OrganizerProfile profile;
        try
        {
            profile = OrganizerProfile.Create(
                command.UserId,
                command.DisplayName,
                command.Bio,
                timeProvider.GetUtcNow().UtcDateTime);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }

        organizerProfileRepository.Add(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return profile.Id;
    }
}
