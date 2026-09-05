using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Shared;
using Event = ReserveFlow.Domain.Catalog.Event;

namespace ReserveFlow.Application.Catalog.CreateEvent;

public sealed class CreateEventCommandHandler(
    IValidator<CreateEventCommand> validator,
    IOrganizerProfileRepository organizerProfileRepository,
    IVenueRepository venueRepository,
    IEventRepository eventRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateEventCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure<Guid>(validation.ToError());
        }

        if (!await organizerProfileRepository.ExistsByIdAsync(command.OrganizerId, cancellationToken))
        {
            return Result.Failure<Guid>(CatalogError.OrganizerNotFound);
        }

        if (!await venueRepository.ExistsByIdAsync(command.VenueId, cancellationToken))
        {
            return Result.Failure<Guid>(CatalogError.VenueNotFound);
        }

        var createdAtUtc = timeProvider.GetUtcNow().UtcDateTime;

        var eventResult = Event.CreateDraft(
            command.OrganizerId,
            command.VenueId,
            command.Title,
            command.Description,
            command.StartAtUtc,
            command.EndAtUtc,
            createdAtUtc);
        if (eventResult.IsFailure)
        {
            return Result.Failure<Guid>(eventResult.Error);
        }

        var @event = eventResult.Value;

        foreach (var ticketType in command.TicketTypes)
        {
            var priceResult = Money.Create(ticketType.PriceAmount, ticketType.Currency);
            if (priceResult.IsFailure)
            {
                return Result.Failure<Guid>(priceResult.Error);
            }

            var ticketTypeResult = @event.AddTicketType(
                ticketType.Name,
                priceResult.Value,
                ticketType.Quota,
                ticketType.SalesStartAtUtc,
                ticketType.SalesEndAtUtc,
                createdAtUtc);
            if (ticketTypeResult.IsFailure)
            {
                return Result.Failure<Guid>(ticketTypeResult.Error);
            }
        }

        eventRepository.Add(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}
