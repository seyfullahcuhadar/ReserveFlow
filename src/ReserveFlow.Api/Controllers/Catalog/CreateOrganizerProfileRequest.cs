namespace ReserveFlow.Api.Controllers.Catalog;

public sealed record CreateOrganizerProfileRequest(
    Guid UserId,
    string DisplayName,
    string? Bio);
