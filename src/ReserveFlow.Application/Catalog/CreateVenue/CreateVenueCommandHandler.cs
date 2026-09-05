using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Shared;

namespace ReserveFlow.Application.Catalog.CreateVenue;

public sealed class CreateVenueCommandHandler(
    IValidator<CreateVenueCommand> validator,
    IVenueRepository venueRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVenueCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateVenueCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure<Guid>(validation.ToError());
        }

        var addressResult = Address.Create(
            command.Street,
            command.City,
            command.Country,
            command.PostalCode);
        if (addressResult.IsFailure)
        {
            return Result.Failure<Guid>(addressResult.Error);
        }

        var venueResult = Venue.Create(
            command.Name,
            addressResult.Value,
            command.Capacity,
            command.TimeZone,
            timeProvider.GetUtcNow().UtcDateTime);
        if (venueResult.IsFailure)
        {
            return Result.Failure<Guid>(venueResult.Error);
        }

        var venue = venueResult.Value;

        venueRepository.Add(venue);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return venue.Id;
    }
}
