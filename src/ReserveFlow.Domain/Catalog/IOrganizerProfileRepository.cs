namespace ReserveFlow.Domain.Catalog;

public interface IOrganizerProfileRepository
{
    Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(Guid organizerId, CancellationToken cancellationToken = default);

    void Add(OrganizerProfile organizerProfile);
}
