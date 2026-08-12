namespace ReserveFlow.Application.Catalog.PublishEvent;

using FluentValidation;

public sealed class PublishEventCommandValidator : AbstractValidator<PublishEventCommand>
{
    public PublishEventCommandValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
    }
}