namespace ReserveFlow.Api.Controllers.Catalog.Dtos;

public sealed record CreateEventTicketTypeRequest(
    string Name,
    decimal PriceAmount,
    string Currency,
    int Quota,
    DateTime SalesStartAtUtc,
    DateTime SalesEndAtUtc);

public sealed record CreateEventRequest(
    Guid OrganizerId,
    Guid VenueId,
    string Title,
    string Description,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    IReadOnlyList<CreateEventTicketTypeRequest> TicketTypes);
