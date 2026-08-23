using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Exceptions;
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

    public static Event CreateDraft(
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
            throw new DomainValidationException("OrganizerId is required.");
        }

        if (venueId == Guid.Empty)
        {
            throw new DomainValidationException("VenueId is required.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainValidationException("Description is required.");
        }

        if (startAtUtc >= endAtUtc)
        {
            throw new DomainValidationException("StartAt must be earlier than EndAt.");
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

    public TicketType AddTicketType(
        string name,
        Money price,
        int quota,
        DateTime salesStartAtUtc,
        DateTime salesEndAtUtc,
        DateTime createdAtUtc)
    {
        EnsureEditable();

        var ticketType = TicketType.Create(name, price, quota, salesStartAtUtc, salesEndAtUtc, createdAtUtc);
        _ticketTypes.Add(ticketType);
        return ticketType;
    }

    public void Publish(DateTime publishedAtUtc)
    {
        if (Status != EventStatus.Draft)
        {
            throw new DomainConflictException("Only draft events can be published.");
        }

        if (!_ticketTypes.Any(t => t.IsActive))
        {
            throw new DomainValidationException("At least one active ticket type is required to publish.");
        }

        if (StartAtUtc <= publishedAtUtc)
        {
            throw new DomainValidationException("An event with a past date cannot be published.");
        }

        Status = EventStatus.Published;
        PublishedAtUtc = publishedAtUtc;
        RaiseDomainEvent(new EventPublishedDomainEvent(Id, OrganizerId, publishedAtUtc));
    }

    public void Cancel(DateTime cancelledAtUtc)
    {
        if (Status is EventStatus.Cancelled or EventStatus.Completed)
        {
            throw new DomainConflictException("Only draft or published events can be cancelled.");
        }

        Status = EventStatus.Cancelled;
        RaiseDomainEvent(new EventCancelledDomainEvent(Id, OrganizerId, cancelledAtUtc));
    }

    private void EnsureEditable()
    {
        if (Status != EventStatus.Draft)
        {
            throw new DomainConflictException("Only draft events can be edited.");
        }
    }
}
