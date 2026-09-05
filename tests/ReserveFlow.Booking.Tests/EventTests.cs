using ReserveFlow.Domain.Catalog;
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

        var result = Event.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  Tech Conference  ",
            "Backend deep dive",
            startAt,
            endAt,
            createdAt);

        Assert.True(result.IsSuccess);
        var @event = result.Value;
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

        var result = Event.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            "Description",
            now.AddHours(2),
            now,
            now);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.StartMustBeEarlierThanEnd, result.Error);
    }

    [Fact]
    public void AddTicketType_ShouldAddActiveTicketType()
    {
        var now = DateTime.UtcNow;
        var @event = DraftEvent(now);

        var result = @event.AddTicketType(
            "VIP",
            Money.Create(250m, "try").Value,
            100,
            now,
            now.AddDays(6),
            now);

        Assert.True(result.IsSuccess);
        var ticketType = result.Value;
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
        var @event = DraftEvent(now);

        var result = @event.AddTicketType(
            "VIP",
            Money.Create(100m).Value,
            50,
            now.AddDays(2),
            now.AddDays(1),
            now);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.SalesWindowInvalid, result.Error);
    }

    [Fact]
    public void Publish_ShouldPublishEvent()
    {
        var now = DateTime.UtcNow;
        var @event = DraftEvent(now, now.AddDays(7), now.AddDays(7).AddHours(3));

        Assert.True(@event.AddTicketType(
            "VIP",
            Money.Create(100m).Value,
            50,
            now,
            now.AddDays(9),
            now).IsSuccess);

        var result = @event.Publish(now);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Published, @event.Status);
        Assert.Equal(now, @event.PublishedAtUtc);
        Assert.Contains(@event.GetDomainEvents(), e => e is EventPublishedDomainEvent);
    }

    [Fact]
    public void Publish_ShouldRejectWhenNoActiveTicketType()
    {
        var now = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var @event = DraftEvent(now, now.AddDays(10), now.AddDays(10).AddHours(2));

        var result = @event.Publish(now);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.ActiveTicketTypeRequiredToPublish, result.Error);
    }

    [Fact]
    public void Publish_ShouldRejectWhenEventDateIsInThePast()
    {
        var now = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var @event = DraftEvent(now.AddDays(-1), now.AddHours(-2), now.AddHours(-1));

        Assert.True(@event.AddTicketType(
            "VIP",
            Money.Create(100m).Value,
            50,
            now.AddDays(-1),
            now,
            now.AddDays(-1)).IsSuccess);

        var result = @event.Publish(now);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.CannotPublishPastEvent, result.Error);
    }

    [Fact]
    public void Publish_ShouldRejectWhenEventIsNotDraft()
    {
        var now = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var @event = DraftEvent(now, now.AddDays(10), now.AddDays(10).AddHours(2));
        Assert.True(@event.AddTicketType("VIP", Money.Create(100m).Value, 50, now, now.AddDays(9), now).IsSuccess);
        Assert.True(@event.Publish(now).IsSuccess);

        var result = @event.Publish(now.AddMinutes(1));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.OnlyDraftEventsCanBePublished, result.Error);
    }

    [Fact]
    public void Cancel_ShouldCancelDraftEvent()
    {
        var now = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var @event = DraftEvent(now, now.AddDays(10), now.AddDays(10).AddHours(2));

        var result = @event.Cancel(now);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Cancelled, @event.Status);
        Assert.Contains(@event.GetDomainEvents(), e => e is EventCancelledDomainEvent);
    }

    [Fact]
    public void Cancel_ShouldCancelPublishedEvent()
    {
        var now = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var @event = DraftEvent(now, now.AddDays(10), now.AddDays(10).AddHours(2));
        Assert.True(@event.AddTicketType("VIP", Money.Create(100m).Value, 50, now, now.AddDays(9), now).IsSuccess);
        Assert.True(@event.Publish(now).IsSuccess);

        var result = @event.Cancel(now.AddMinutes(1));

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Cancelled, @event.Status);
        Assert.Contains(@event.GetDomainEvents(), e => e is EventCancelledDomainEvent);
    }

    [Fact]
    public void Cancel_ShouldRejectWhenEventIsAlreadyCancelled()
    {
        var now = new DateTime(2026, 8, 5, 12, 0, 0, DateTimeKind.Utc);
        var @event = DraftEvent(now, now.AddDays(10), now.AddDays(10).AddHours(2));
        Assert.True(@event.Cancel(now).IsSuccess);

        var result = @event.Cancel(now.AddMinutes(1));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogError.OnlyDraftOrPublishedCanBeCancelled, result.Error);
    }

    private static Event DraftEvent(DateTime createdAt) =>
        DraftEvent(createdAt, createdAt.AddDays(7), createdAt.AddDays(7).AddHours(3));

    private static Event DraftEvent(DateTime createdAt, DateTime startAt, DateTime endAt) =>
        Event.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Title",
            "Description",
            startAt,
            endAt,
            createdAt).Value;
}
