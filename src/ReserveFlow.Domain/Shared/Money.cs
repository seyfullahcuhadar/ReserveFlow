using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Exceptions;

namespace ReserveFlow.Domain.Shared;

public sealed class Money : ValueObject
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    // EF Core materialization
    private Money()
    {
        Currency = null!;
    }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; }

    public static Money Create(decimal amount, string currency = "TRY")
    {
        if (amount < 0)
        {
            throw new DomainValidationException("Amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainValidationException("Currency is required.");
        }

        var normalized = currency.Trim().ToUpperInvariant();
        if (normalized.Length is < 3 or > 3)
        {
            throw new DomainValidationException("Currency must be a 3-letter ISO code.");
        }

        return new Money(amount, normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
