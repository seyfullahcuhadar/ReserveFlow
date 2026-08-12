using ReserveFlow.Application.Exceptions;
using ReserveFlow.Domain.Exceptions;
using ValidationException = ReserveFlow.Application.Exceptions.ValidationException;

namespace ReserveFlow.Application.Messaging;

internal sealed class ExceptionMappingCommandHandler<TCommand>(
    ICommandHandler<TCommand> inner) : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await inner.HandleAsync(command, cancellationToken);
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

internal sealed class ExceptionMappingCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner) : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        try
        {
            return await inner.HandleAsync(command, cancellationToken);
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
