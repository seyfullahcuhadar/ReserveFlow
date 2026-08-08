using Microsoft.EntityFrameworkCore;
using ReserveFlow.Domain.Abstractions;
using ReserveFlow.Domain.Catalog;
using ReserveFlow.Domain.Users;

namespace ReserveFlow.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options),IUnitOfWork
{
    public DbSet<User> Users => Set<User>();

    public DbSet<OrganizerProfile> OrganizerProfiles => Set<OrganizerProfile>();

    public DbSet<Venue> Venues => Set<Venue>();

    public DbSet<Event> Events => Set<Event>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
