using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Exceptions;
using ReserveFlow.Domain.Shared;

namespace ReserveFlow.Domain.Catalog;

public sealed class Venue : AggregateRoot
{
    private Venue(
        Guid id,
        string name,
        Address address,
        int capacity,
        string timeZone,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        Name = name;
        Address = address;
        Capacity = capacity;
        TimeZone = timeZone;
    }

    private Venue()
    {
    }

    public string Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public int Capacity { get; private set; }

    public string TimeZone { get; private set; } = null!;

    public static Venue Create(
        string name,
        Address address,
        int capacity,
        string timeZone,
        DateTime createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Venue name is required.");
        }

        if (capacity <= 0)
        {
            throw new DomainValidationException("Venue capacity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(timeZone))
        {
            throw new DomainValidationException("Time zone is required.");
        }

        var venue = new Venue(
            Guid.NewGuid(),
            name.Trim(),
            address,
            capacity,
            timeZone.Trim(),
            createdAtUtc);

        venue.RaiseDomainEvent(
            new VenueCreatedDomainEvent(venue.Id, createdAtUtc));

        return venue;
    }
}
