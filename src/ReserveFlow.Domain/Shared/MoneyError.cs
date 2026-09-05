using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Shared;

public static class MoneyError
{
    public static readonly Error AmountCannotBeNegative = new(
        "Shared.AmountCannotBeNegative",
        "Amount cannot be negative.",
        ErrorType.Validation);

    public static readonly Error CurrencyRequired = new(
        "Shared.CurrencyRequired",
        "Currency is required.",
        ErrorType.Validation);

    public static readonly Error CurrencyMustBeIsoCode = new(
        "Shared.CurrencyMustBeIsoCode",
        "Currency must be a 3-letter ISO code.",
        ErrorType.Validation);
}
