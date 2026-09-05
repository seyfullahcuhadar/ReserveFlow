using ReserveFlow.Domain.Abstractions;

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

    public static Result<Money> Create(decimal amount, string currency = "TRY")
    {
        if (amount < 0)
        {
            return Result.Failure<Money>(MoneyError.AmountCannotBeNegative);
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            return Result.Failure<Money>(MoneyError.CurrencyRequired);
        }

        var normalized = currency.Trim().ToUpperInvariant();
        if (normalized.Length is < 3 or > 3)
        {
            return Result.Failure<Money>(MoneyError.CurrencyMustBeIsoCode);
        }

        return new Money(amount, normalized);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
