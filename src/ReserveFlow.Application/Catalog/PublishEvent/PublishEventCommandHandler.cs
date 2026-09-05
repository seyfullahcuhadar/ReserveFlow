using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Application.Catalog.PublishEvent;

public sealed class PublishEventCommandHandler(
    IValidator<PublishEventCommand> validator,
    IEventRepository eventRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<PublishEventCommand>
{
    public async Task<Result> HandleAsync(PublishEventCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Result.Failure(validation.ToError());
        }

        var @event = await eventRepository.GetByIdAsync(command.EventId, cancellationToken);
        if (@event is null)
        {
            return Result.Failure(CatalogError.EventNotFound);
        }

        var publishResult = @event.Publish(timeProvider.GetUtcNow().UtcDateTime);
        if (publishResult.IsFailure)
        {
            return publishResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
