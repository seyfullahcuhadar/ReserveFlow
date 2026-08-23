using FluentValidation;

namespace ReserveFlow.Application.Catalog.CancelEvent;

public sealed class CancelEventCommandValidator : AbstractValidator<CancelEventCommand>
{
    public CancelEventCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}
