namespace ReserveFlow.Domain.Abstractions;

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    Conflict = 2,
    Unauthorized = 3
}
