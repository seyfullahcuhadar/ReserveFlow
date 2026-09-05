using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Shared;

namespace ReserveFlow.Domain.Catalog;

public sealed class TicketType : Entity
{
    private TicketType(
        Guid id,
        string name,
        Money price,
        int quota,
        DateTime salesStartAtUtc,
        DateTime salesEndAtUtc,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        Name = name;
        Price = price;
        Quota = quota;
        SoldCount = 0;
        SalesStartAtUtc = salesStartAtUtc;
        SalesEndAtUtc = salesEndAtUtc;
        IsActive = true;
    }

    private TicketType()
    {
    }

    public string Name { get; private set; } = null!;

    public Money Price { get; private set; } = null!;

    public int Quota { get; private set; }

    public int SoldCount { get; private set; }

    public DateTime SalesStartAtUtc { get; private set; }

    public DateTime SalesEndAtUtc { get; private set; }

    public bool IsActive { get; private set; }

    internal static Result<TicketType> Create(
        string name,
        Money price,
        int quota,
        DateTime salesStartAtUtc,
        DateTime salesEndAtUtc,
        DateTime createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(price);

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<TicketType>(CatalogError.TicketTypeNameRequired);
        }

        if (quota <= 0)
        {
            return Result.Failure<TicketType>(CatalogError.QuotaMustBePositive);
        }

        if (salesStartAtUtc >= salesEndAtUtc)
        {
            return Result.Failure<TicketType>(CatalogError.SalesWindowInvalid);
        }

        return new TicketType(
            Guid.NewGuid(),
            name.Trim(),
            price,
            quota,
            salesStartAtUtc,
            salesEndAtUtc,
            createdAtUtc);
    }
}
