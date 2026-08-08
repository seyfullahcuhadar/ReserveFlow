using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Catalog;

public sealed record VenueCreatedDomainEvent(
    Guid VenueId,
    DateTime OccurredOnUtc) : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
}
