using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Catalog;

public static class CatalogError
{
    public static readonly Error OrganizerNotFound = new(
        "Catalog.OrganizerNotFound",
        "Organizer profile was not found.",
        ErrorType.Validation);

    public static readonly Error VenueNotFound = new(
        "Catalog.VenueNotFound",
        "Venue was not found.",
        ErrorType.Validation);

    public static readonly Error EventNotFound = new(
        "Catalog.EventNotFound",
        "Event was not found.",
        ErrorType.Validation);

    public static readonly Error UserNotFound = new(
        "Catalog.UserNotFound",
        "User was not found.",
        ErrorType.Validation);

    public static readonly Error OrganizerProfileAlreadyExists = new(
        "Catalog.OrganizerProfileAlreadyExists",
        "Organizer profile already exists for this user.",
        ErrorType.Conflict);

    public static readonly Error VenueNameRequired = new(
        "Catalog.VenueNameRequired",
        "Venue name is required.",
        ErrorType.Validation);

    public static readonly Error VenueCapacityMustBePositive = new(
        "Catalog.VenueCapacityMustBePositive",
        "Venue capacity must be greater than zero.",
        ErrorType.Validation);

    public static readonly Error TimeZoneRequired = new(
        "Catalog.TimeZoneRequired",
        "Time zone is required.",
        ErrorType.Validation);

    public static readonly Error OrganizerIdRequired = new(
        "Catalog.OrganizerIdRequired",
        "OrganizerId is required.",
        ErrorType.Validation);

    public static readonly Error VenueIdRequired = new(
        "Catalog.VenueIdRequired",
        "VenueId is required.",
        ErrorType.Validation);

    public static readonly Error TitleRequired = new(
        "Catalog.TitleRequired",
        "Title is required.",
        ErrorType.Validation);

    public static readonly Error DescriptionRequired = new(
        "Catalog.DescriptionRequired",
        "Description is required.",
        ErrorType.Validation);

    public static readonly Error StartMustBeEarlierThanEnd = new(
        "Catalog.StartMustBeEarlierThanEnd",
        "StartAt must be earlier than EndAt.",
        ErrorType.Validation);

    public static readonly Error TicketTypeNameRequired = new(
        "Catalog.TicketTypeNameRequired",
        "Ticket type name is required.",
        ErrorType.Validation);

    public static readonly Error QuotaMustBePositive = new(
        "Catalog.QuotaMustBePositive",
        "Quota must be greater than zero.",
        ErrorType.Validation);

    public static readonly Error SalesWindowInvalid = new(
        "Catalog.SalesWindowInvalid",
        "SalesStartAt must be earlier than SalesEndAt.",
        ErrorType.Validation);

    public static readonly Error OnlyDraftEventsCanBePublished = new(
        "Catalog.OnlyDraftEventsCanBePublished",
        "Only draft events can be published.",
        ErrorType.Conflict);

    public static readonly Error ActiveTicketTypeRequiredToPublish = new(
        "Catalog.ActiveTicketTypeRequiredToPublish",
        "At least one active ticket type is required to publish.",
        ErrorType.Validation);

    public static readonly Error CannotPublishPastEvent = new(
        "Catalog.CannotPublishPastEvent",
        "An event with a past date cannot be published.",
        ErrorType.Validation);

    public static readonly Error OnlyDraftOrPublishedCanBeCancelled = new(
        "Catalog.OnlyDraftOrPublishedCanBeCancelled",
        "Only draft or published events can be cancelled.",
        ErrorType.Conflict);

    public static readonly Error OnlyDraftEventsCanBeEdited = new(
        "Catalog.OnlyDraftEventsCanBeEdited",
        "Only draft events can be edited.",
        ErrorType.Conflict);
}
