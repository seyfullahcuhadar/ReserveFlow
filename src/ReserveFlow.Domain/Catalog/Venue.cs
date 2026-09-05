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

    public static Result<Venue> Create(
        string name,
        Address address,
        int capacity,
        string timeZone,
        DateTime createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Venue>(CatalogError.VenueNameRequired);
        }

        if (capacity <= 0)
        {
            return Result.Failure<Venue>(CatalogError.VenueCapacityMustBePositive);
        }

        if (string.IsNullOrWhiteSpace(timeZone))
        {
            return Result.Failure<Venue>(CatalogError.TimeZoneRequired);
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
