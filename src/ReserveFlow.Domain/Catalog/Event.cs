using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Shared;

namespace ReserveFlow.Domain.Catalog;

public sealed class Event : AggregateRoot
{
    private readonly List<TicketType> _ticketTypes = [];

    private Event(
        Guid id,
        Guid organizerId,
        Guid venueId,
        string title,
        string description,
        DateTime startAtUtc,
        DateTime endAtUtc,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        OrganizerId = organizerId;
        VenueId = venueId;
        Title = title;
        Description = description;
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        Status = EventStatus.Draft;
    }

    private Event()
    {
    }

    public Guid OrganizerId { get; private set; }

    public Guid VenueId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public DateTime StartAtUtc { get; private set; }

    public DateTime EndAtUtc { get; private set; }

    public EventStatus Status { get; private set; }

    public DateTime? PublishedAtUtc { get; private set; }

    public IReadOnlyList<TicketType> TicketTypes => _ticketTypes;

    public static Result<Event> CreateDraft(
        Guid organizerId,
        Guid venueId,
        string title,
        string description,
        DateTime startAtUtc,
        DateTime endAtUtc,
        DateTime createdAtUtc)
    {
        if (organizerId == Guid.Empty)
        {
            return Result.Failure<Event>(CatalogError.OrganizerIdRequired);
        }

        if (venueId == Guid.Empty)
        {
            return Result.Failure<Event>(CatalogError.VenueIdRequired);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Event>(CatalogError.TitleRequired);
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Failure<Event>(CatalogError.DescriptionRequired);
        }

        if (startAtUtc >= endAtUtc)
        {
            return Result.Failure<Event>(CatalogError.StartMustBeEarlierThanEnd);
        }

        var @event = new Event(
            Guid.NewGuid(),
            organizerId,
            venueId,
            title.Trim(),
            description.Trim(),
            startAtUtc,
            endAtUtc,
            createdAtUtc);

        @event.RaiseDomainEvent(
            new EventCreatedDomainEvent(@event.Id, @event.OrganizerId, createdAtUtc));

        return @event;
    }

    public Result<TicketType> AddTicketType(
        string name,
        Money price,
        int quota,
        DateTime salesStartAtUtc,
        DateTime salesEndAtUtc,
        DateTime createdAtUtc)
    {
        var editable = EnsureEditable();
        if (editable.IsFailure)
        {
            return Result.Failure<TicketType>(editable.Error);
        }

        var ticketTypeResult = TicketType.Create(name, price, quota, salesStartAtUtc, salesEndAtUtc, createdAtUtc);
        if (ticketTypeResult.IsFailure)
        {
            return Result.Failure<TicketType>(ticketTypeResult.Error);
        }

        var ticketType = ticketTypeResult.Value;
        _ticketTypes.Add(ticketType);
        return ticketType;
    }

    public Result Publish(DateTime publishedAtUtc)
    {
        if (Status != EventStatus.Draft)
        {
            return Result.Failure(CatalogError.OnlyDraftEventsCanBePublished);
        }

        if (!_ticketTypes.Any(t => t.IsActive))
        {
            return Result.Failure(CatalogError.ActiveTicketTypeRequiredToPublish);
        }

        if (StartAtUtc <= publishedAtUtc)
        {
            return Result.Failure(CatalogError.CannotPublishPastEvent);
        }

        Status = EventStatus.Published;
        PublishedAtUtc = publishedAtUtc;
        RaiseDomainEvent(new EventPublishedDomainEvent(Id, OrganizerId, publishedAtUtc));
        return Result.Success();
    }

    public Result Cancel(DateTime cancelledAtUtc)
    {
        if (Status is EventStatus.Cancelled or EventStatus.Completed)
        {
            return Result.Failure(CatalogError.OnlyDraftOrPublishedCanBeCancelled);
        }

        Status = EventStatus.Cancelled;
        RaiseDomainEvent(new EventCancelledDomainEvent(Id, OrganizerId, cancelledAtUtc));
        return Result.Success();
    }

    private Result EnsureEditable()
    {
        if (Status != EventStatus.Draft)
        {
            return Result.Failure(CatalogError.OnlyDraftEventsCanBeEdited);
        }

        return Result.Success();
    }
}
