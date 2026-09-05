using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Shared;

namespace ReserveFlow.Booking.Tests;

public class VenueTests
{
    [Fact]
    public void Create_ShouldCreateVenueWithDomainEvent()
    {
        var createdAt = new DateTime(2026, 8, 8, 12, 0, 0, DateTimeKind.Utc);
        var address = Address.Create("Main St 1", "Istanbul", "TR", "34000").Value;

        var result = Venue.Create(
            "  Congress Center  ",
            address,
            500,
            "Europe/Istanbul",
            createdAt);

        Assert.True(result.IsSuccess);
        var venue = result.Value;
        Assert.Equal("Congress Center", venue.Name);
        Assert.Equal(500, venue.Capacity);
        Assert.Equal("Europe/Istanbul", venue.TimeZone);
        Assert.Equal(createdAt, venue.CreatedAtUtc);
        Assert.Single(venue.GetDomainEvents());
        Assert.IsType<VenueCreatedDomainEvent>(venue.GetDomainEvents()[0]);
    }

    [Fact]
    public void Create_ShouldRejectInvalidCapacity()
    {
        var address = Address.Create("Main St 1", "Istanbul", "TR").Value;

        var result = Venue.Create("Hall", address, 0, "Europe/Istanbul", DateTime.UtcNow);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.VenueCapacityMustBePositive, result.Error);
    }
}
