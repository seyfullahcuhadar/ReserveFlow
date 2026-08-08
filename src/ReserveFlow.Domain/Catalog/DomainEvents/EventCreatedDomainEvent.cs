using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Catalog;

public sealed record EventCreatedDomainEvent(
    Guid EventId,
    Guid OrganizerId,
    DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
