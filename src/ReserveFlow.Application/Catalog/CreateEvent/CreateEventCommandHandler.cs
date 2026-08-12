using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Shared;
using ValidationException = ReserveFlow.Application.Exceptions.ValidationException;
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
    public async Task<Guid> HandleAsync(CreateEventCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (!await organizerProfileRepository.ExistsByIdAsync(command.OrganizerId, cancellationToken))
        {
            throw new ValidationException("Organizer profile was not found.");
        }

        if (!await venueRepository.ExistsByIdAsync(command.VenueId, cancellationToken))
        {
            throw new ValidationException("Venue was not found.");
        }

        var createdAtUtc = timeProvider.GetUtcNow().UtcDateTime;

        var @event = Event.CreateDraft(
            command.OrganizerId,
            command.VenueId,
            command.Title,
            command.Description,
            command.StartAtUtc,
            command.EndAtUtc,
            createdAtUtc);

        foreach (var ticketType in command.TicketTypes)
        {
            var price = Money.Create(ticketType.PriceAmount, ticketType.Currency);
            @event.AddTicketType(
                ticketType.Name,
                price,
                ticketType.Quota,
                ticketType.SalesStartAtUtc,
                ticketType.SalesEndAtUtc,
                createdAtUtc);
        }

        eventRepository.Add(@event);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return @event.Id;
    }
}
