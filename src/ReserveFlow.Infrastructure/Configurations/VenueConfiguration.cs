using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Infrastructure.Configurations;

internal sealed class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedNever();

        builder.Property(v => v.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(v => v.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

        builder.Property(v => v.TimeZone)
            .HasColumnName("time_zone")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(v => v.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.OwnsOne(v => v.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("street")
                .HasMaxLength(300)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("city")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("country")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.PostalCode)
                .HasColumnName("postal_code")
                .HasMaxLength(32);
        });

        builder.Navigation(v => v.Address)
            .IsRequired();
    }
}
