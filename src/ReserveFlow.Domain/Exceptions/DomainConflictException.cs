namespace ReserveFlow.Domain.Exceptions;

public sealed class DomainConflictException : DomainException
{
    public DomainConflictException(string message)
        : base(message)
    {
    }
}
