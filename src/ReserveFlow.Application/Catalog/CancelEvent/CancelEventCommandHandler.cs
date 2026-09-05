using FluentValidation;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Validation;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Application.Catalog.CancelEvent;

public sealed class CancelEventCommandHandler(
    IValidator<CancelEventCommand> validator,
    IEventRepository eventRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CancelEventCommand>
{
    public async Task<Result> HandleAsync(CancelEventCommand command, CancellationToken cancellationToken)
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

        var cancelResult = @event.Cancel(timeProvider.GetUtcNow().UtcDateTime);
        if (cancelResult.IsFailure)
        {
            return cancelResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
