using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Application.Catalog.CreateVenue;

public sealed record CreateVenueCommand(
    string Name,
    string Street,
    string City,
    string Country,
    string? PostalCode,
    int Capacity,
    string TimeZone) : ICommand<Guid>;
