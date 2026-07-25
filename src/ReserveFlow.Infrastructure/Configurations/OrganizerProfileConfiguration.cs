using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Infrastructure.Configurations;

internal sealed class OrganizerProfileConfiguration : IEntityTypeConfiguration<OrganizerProfile>
{
    public void Configure(EntityTypeBuilder<OrganizerProfile> builder)
    {
        builder.ToTable("organizer_profiles");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedNever();

        builder.Property(o => o.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(o => o.UserId)
            .IsUnique();

        builder.Property(o => o.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Bio)
            .HasColumnName("bio")
            .HasMaxLength(2000);

        builder.Property(o => o.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();
    }
}
