using FluentValidation;

namespace ReserveFlow.Application.Catalog.CreateEvent;

public sealed class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(x => x.OrganizerId)
            .NotEmpty();

        RuleFor(x => x.VenueId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.StartAtUtc)
            .LessThan(x => x.EndAtUtc)
            .WithMessage("StartAt must be earlier than EndAt.");

        RuleFor(x => x.TicketTypes)
            .NotEmpty()
            .WithMessage("At least one ticket type is required.");

        RuleForEach(x => x.TicketTypes).ChildRules(ticket =>
        {
            ticket.RuleFor(t => t.Name)
                .NotEmpty()
                .MaximumLength(100);

            ticket.RuleFor(t => t.PriceAmount)
                .GreaterThanOrEqualTo(0);

            ticket.RuleFor(t => t.Currency)
                .NotEmpty()
                .Length(3);

            ticket.RuleFor(t => t.Quota)
                .GreaterThan(0);

            ticket.RuleFor(t => t.SalesStartAtUtc)
                .LessThan(t => t.SalesEndAtUtc)
                .WithMessage("SalesStartAt must be earlier than SalesEndAt.");
        });
    }
}
