using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Infrastructure.Configurations;

internal sealed class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.OrganizerId)
            .HasColumnName("organizer_id")
            .IsRequired();

        builder.HasIndex(e => e.OrganizerId);

        builder.Property(e => e.VenueId)
            .HasColumnName("venue_id")
            .IsRequired();

        builder.HasIndex(e => e.VenueId);

        builder.Property(e => e.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(e => e.StartAtUtc)
            .HasColumnName("start_at_utc")
            .IsRequired();

        builder.Property(e => e.EndAtUtc)
            .HasColumnName("end_at_utc")
            .IsRequired();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(e => e.PublishedAtUtc)
            .HasColumnName("published_at_utc");

        builder.Property(e => e.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Ignore(e => e.TicketTypes);

        builder.OwnsMany<TicketType>("_ticketTypes", ticket =>
        {
            ticket.ToTable("ticket_types");

            ticket.WithOwner()
                .HasForeignKey("event_id");

            ticket.HasKey(t => t.Id);

            ticket.Property(t => t.Id)
                .ValueGeneratedNever();

            ticket.Property(t => t.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            ticket.Property(t => t.Quota)
                .HasColumnName("quota")
                .IsRequired();

            ticket.Property(t => t.SoldCount)
                .HasColumnName("sold_count")
                .IsRequired();

            ticket.Property(t => t.SalesStartAtUtc)
                .HasColumnName("sales_start_at_utc")
                .IsRequired();

            ticket.Property(t => t.SalesEndAtUtc)
                .HasColumnName("sales_end_at_utc")
                .IsRequired();

            ticket.Property(t => t.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            ticket.OwnsOne(t => t.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("price_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("price_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            ticket.Navigation(t => t.Price)
                .IsRequired();
        });
    }
}
