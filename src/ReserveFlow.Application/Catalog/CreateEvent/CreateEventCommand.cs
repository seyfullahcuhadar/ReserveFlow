using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Application.Catalog.CreateEvent;

public sealed record CreateEventTicketTypeDto(
    string Name,
    decimal PriceAmount,
    string Currency,
    int Quota,
    DateTime SalesStartAtUtc,
    DateTime SalesEndAtUtc);

public sealed record CreateEventCommand(
    Guid OrganizerId,
    Guid VenueId,
    string Title,
    string Description,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    IReadOnlyList<CreateEventTicketTypeDto> TicketTypes) : ICommand<Guid>;
