using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ValidationException = ReserveFlow.Application.Exceptions.ValidationException;

namespace ReserveFlow.Application.Catalog.PublishEvent;

public sealed class PublishEventCommandHandler(
    IValidator<PublishEventCommand> validator,
    IEventRepository eventRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<PublishEventCommand>
{
    public async Task HandleAsync(PublishEventCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var @event = await eventRepository.GetByIdAsync(command.EventId, cancellationToken)
            ?? throw new ValidationException("Event was not found.");

        @event.Publish(timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
