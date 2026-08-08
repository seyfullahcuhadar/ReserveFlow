namespace ReserveFlow.Api.Controllers.Catalog;

public sealed record CreateVenueRequest(
    string Name,
    string Street,
    string City,
    string Country,
    string? PostalCode,
    int Capacity,
    string TimeZone);
