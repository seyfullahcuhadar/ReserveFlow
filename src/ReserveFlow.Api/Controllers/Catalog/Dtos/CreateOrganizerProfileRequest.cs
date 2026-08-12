namespace ReserveFlow.Api.Controllers.Catalog.Dtos;

public sealed record CreateOrganizerProfileRequest(
    Guid UserId,
    string DisplayName,
    string? Bio);
