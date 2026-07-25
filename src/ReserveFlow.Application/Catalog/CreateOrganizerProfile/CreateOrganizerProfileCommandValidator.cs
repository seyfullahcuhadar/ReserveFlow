using FluentValidation;

namespace ReserveFlow.Application.Catalog.CreateOrganizerProfile;

public sealed class CreateOrganizerProfileCommandValidator : AbstractValidator<CreateOrganizerProfileCommand>
{
    public CreateOrganizerProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .When(x => x.Bio is not null);
    }
}
