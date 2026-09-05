using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Shared;

public static class AddressError
{
    public static readonly Error StreetRequired = new(
        "Shared.StreetRequired",
        "Street is required.",
        ErrorType.Validation);

    public static readonly Error CityRequired = new(
        "Shared.CityRequired",
        "City is required.",
        ErrorType.Validation);

    public static readonly Error CountryRequired = new(
        "Shared.CountryRequired",
        "Country is required.",
        ErrorType.Validation);
}
