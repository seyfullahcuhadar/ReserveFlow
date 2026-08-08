using ReserveFlow.Domain.Abstractions;
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
        : base(id)
    {
        Name = name;
        Address = address;
        Capacity = capacity;
        TimeZone = timeZone;
        CreatedAtUtc = createdAtUtc;
    }

    private Venue()
    {
    }

    public string Name { get; private set; } = null!;

    public Address Address { get; private set; } = null!;

    public int Capacity { get; private set; }

    public string TimeZone { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

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
            throw new ArgumentException("Venue name is required.", nameof(name));
        }

        if (capacity <= 0)
        {
            throw new ArgumentException("Venue capacity must be greater than zero.", nameof(capacity));
        }

        if (string.IsNullOrWhiteSpace(timeZone))
        {
            throw new ArgumentException("Time zone is required.", nameof(timeZone));
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
