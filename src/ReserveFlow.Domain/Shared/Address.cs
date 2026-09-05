using ReserveFlow.Domain.Abstractions;

namespace ReserveFlow.Domain.Shared;

public sealed class Address : ValueObject
{
    private Address(string street, string city, string country, string? postalCode)
    {
        Street = street;
        City = city;
        Country = country;
        PostalCode = postalCode;
    }

    // EF Core materialization
    private Address()
    {
        Street = null!;
        City = null!;
        Country = null!;
    }

    public string Street { get; private set; }

    public string City { get; private set; }

    public string Country { get; private set; }

    public string? PostalCode { get; private set; }

    public static Result<Address> Create(string street, string city, string country, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result.Failure<Address>(AddressError.StreetRequired);
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Result.Failure<Address>(AddressError.CityRequired);
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            return Result.Failure<Address>(AddressError.CountryRequired);
        }

        return new Address(
            street.Trim(),
            city.Trim(),
            country.Trim(),
            string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return Country;
        yield return PostalCode;
    }
}
