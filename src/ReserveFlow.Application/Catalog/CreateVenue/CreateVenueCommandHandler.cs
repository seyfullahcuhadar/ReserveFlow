using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Shared;
using ValidationException = ReserveFlow.Application.Exceptions.ValidationException;

namespace ReserveFlow.Application.Catalog.CreateVenue;

public sealed class CreateVenueCommandHandler(
    IValidator<CreateVenueCommand> validator,
    IVenueRepository venueRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVenueCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateVenueCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        Venue venue;
        try
        {
            var address = Address.Create(
                command.Street,
                command.City,
                command.Country,
                command.PostalCode);

            venue = Venue.Create(
                command.Name,
                address,
                command.Capacity,
                command.TimeZone,
                timeProvider.GetUtcNow().UtcDateTime);
        }
        catch (ArgumentException ex)
        {
            throw new ValidationException(ex.Message);
        }

        venueRepository.Add(venue);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}
