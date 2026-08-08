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
        DateTime salesEndAtUtc)
        : base(id)
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

    internal static TicketType Create(
        string name,
        Money price,
        int quota,
        DateTime salesStartAtUtc,
        DateTime salesEndAtUtc)
    {
        ArgumentNullException.ThrowIfNull(price);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ticket type name is required.", nameof(name));
        }

        if (quota <= 0)
        {
            throw new ArgumentException("Quota must be greater than zero.", nameof(quota));
        }

        if (salesStartAtUtc >= salesEndAtUtc)
        {
            throw new ArgumentException("SalesStartAt must be earlier than SalesEndAt.", nameof(salesStartAtUtc));
        }

        return new TicketType(
            Guid.NewGuid(),
            name.Trim(),
            price,
            quota,
            salesStartAtUtc,
            salesEndAtUtc);
    }
}
