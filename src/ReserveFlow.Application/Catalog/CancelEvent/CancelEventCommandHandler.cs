using FluentValidation;
using ReserveFlow.Application.Exceptions;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Exceptions;
using ValidationException = ReserveFlow.Application.Exceptions.ValidationException;

namespace ReserveFlow.Application.Catalog.CancelEvent;

public sealed class CancelEventCommandHandler(
    IValidator<CancelEventCommand> validator,
    IEventRepository eventRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) : ICommandHandler<CancelEventCommand>
{
    public async Task HandleAsync(CancelEventCommand command, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        var @event = await eventRepository.GetByIdAsync(command.EventId, cancellationToken)
            ?? throw new ValidationException("Event was not found.");

        // Wolverine InvokeAsync decorator'ı atladığı için domain → application mapping burada.
        // Tüm handler'lar Wolverine'e geçince middleware'e taşınacak.
        try
        {
            @event.Cancel(timeProvider.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DomainValidationException ex)
        {
            throw new ValidationException(ex.Message);
        }
        catch (DomainConflictException ex)
        {
            throw new ConflictException(ex.Message);
        }
    }
}
