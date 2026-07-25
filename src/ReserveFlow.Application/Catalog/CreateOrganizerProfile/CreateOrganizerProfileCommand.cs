using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Application.Catalog.CreateOrganizerProfile;

public sealed record CreateOrganizerProfileCommand(
    Guid UserId,
    string DisplayName,
    string? Bio) : ICommand<Guid>;
