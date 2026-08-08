namespace ReserveFlow.Domain.Catalog;

public interface IVenueRepository
{
    Task<bool> ExistsByIdAsync(Guid venueId, CancellationToken cancellationToken = default);

    void Add(Venue venue);
}
