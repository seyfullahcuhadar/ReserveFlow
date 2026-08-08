using FluentValidation;

namespace ReserveFlow.Application.Catalog.CreateVenue;

public sealed class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Street)
            .NotEmpty()
            .MaximumLength(300);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .MaximumLength(32)
            .When(x => x.PostalCode is not null);

        RuleFor(x => x.Capacity)
            .GreaterThan(0);

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .MaximumLength(100);
    }
}
