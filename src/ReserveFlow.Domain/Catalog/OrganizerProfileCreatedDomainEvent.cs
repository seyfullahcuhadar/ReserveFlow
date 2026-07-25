using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Catalog;

public sealed record OrganizerProfileCreatedDomainEvent(
    Guid OrganizerId,
    Guid UserId,
    DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
