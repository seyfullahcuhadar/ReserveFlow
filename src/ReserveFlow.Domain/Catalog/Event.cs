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
        : base(id)
    {
        OrganizerId = organizerId;
        VenueId = venueId;
        Title = title;
        Description = description;
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        Status = EventStatus.Draft;
        CreatedAtUtc = createdAtUtc;
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

    public DateTime CreatedAtUtc { get; private set; }

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
            throw new ArgumentException("OrganizerId is required.", nameof(organizerId));
        }

        if (venueId == Guid.Empty)
        {
            throw new ArgumentException("VenueId is required.", nameof(venueId));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (startAtUtc >= endAtUtc)
        {
            throw new ArgumentException("StartAt must be earlier than EndAt.", nameof(startAtUtc));
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
        DateTime salesEndAtUtc)
    {
        EnsureEditable();

        var ticketType = TicketType.Create(name, price, quota, salesStartAtUtc, salesEndAtUtc);
        _ticketTypes.Add(ticketType);
        return ticketType;
    }

    private void EnsureEditable()
    {
        if (Status != EventStatus.Draft)
        {
            throw new InvalidOperationException("Only draft events can be edited.");
        }
    }
}
