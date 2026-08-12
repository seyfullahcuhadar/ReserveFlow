using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Exceptions;
using ReserveFlow.Domain.Shared;
using Event = ReserveFlow.Domain.Catalog.Event;

namespace ReserveFlow.Booking.Tests;

public class EventTests
{
    [Fact]
    public void CreateDraft_ShouldCreateDraftEventWithDomainEvent()
    {
        var createdAt = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var startAt = createdAt.AddDays(10);
        var endAt = startAt.AddHours(2);

        var @event = Event.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  Tech Conference  ",
            "Backend deep dive",
            startAt,
            endAt,
            createdAt);

        Assert.Equal(EventStatus.Draft, @event.Status);
        Assert.Equal("Tech Conference", @event.Title);
        Assert.Equal(createdAt, @event.CreatedAtUtc);
        Assert.Empty(@event.TicketTypes);
        Assert.Single(@event.GetDomainEvents());
        Assert.IsType<EventCreatedDomainEvent>(@event.GetDomainEvents()[0]);
    }

    [Fact]
    public void CreateDraft_ShouldRejectInvalidDateRange()
    {
        var now = DateTime.UtcNow;

        Assert.Throws<DomainValidationException>(() =>
            Event.CreateDraft(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Title",
                "Description",
                now.AddHours(2),
                now,
                now));
    }

    [Fact]
    public void AddTicketType_ShouldAddActiveTicketType()
    {
        var now = DateTime.UtcNow;
        var @event = Event.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            "Description",
            now.AddDays(7),
            now.AddDays(7).AddHours(3),
            now);

        var ticketType = @event.AddTicketType(
            "VIP",
            Money.Create(250m, "try"),
            100,
            now,
            now.AddDays(6),
            now);

        Assert.Single(@event.TicketTypes);
        Assert.Equal("VIP", ticketType.Name);
        Assert.Equal(250m, ticketType.Price.Amount);
        Assert.Equal("TRY", ticketType.Price.Currency);
        Assert.Equal(0, ticketType.SoldCount);
        Assert.True(ticketType.IsActive);
        Assert.Equal(now, ticketType.CreatedAtUtc);
    }

    [Fact]
    public void AddTicketType_ShouldRejectInvalidSalesWindow()
    {
        var now = DateTime.UtcNow;
        var @event = Event.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            "Description",
            now.AddDays(7),
            now.AddDays(7).AddHours(3),
            now);

        Assert.Throws<DomainValidationException>(() =>
            @event.AddTicketType(
                "VIP",
                Money.Create(100m),
                50,
                now.AddDays(2),
                now.AddDays(1),
                now));
    }
}
