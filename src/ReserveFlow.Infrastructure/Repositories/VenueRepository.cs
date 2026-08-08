using Microsoft.EntityFrameworkCore;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Infrastructure.Repositories;

public sealed class VenueRepository(ApplicationDbContext dbContext) : IVenueRepository
{
    public Task<bool> ExistsByIdAsync(Guid venueId, CancellationToken cancellationToken = default) =>
        dbContext.Venues.AnyAsync(v => v.Id == venueId, cancellationToken);

    public void Add(Venue venue) =>
        dbContext.Venues.Add(venue);
}
