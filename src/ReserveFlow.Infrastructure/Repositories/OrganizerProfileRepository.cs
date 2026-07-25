using Microsoft.EntityFrameworkCore;
using ReserveFlow.Domain.Catalog;

namespace ReserveFlow.Infrastructure.Repositories;

public sealed class OrganizerProfileRepository(ApplicationDbContext dbContext) : IOrganizerProfileRepository
{
    public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.OrganizerProfiles.AnyAsync(o => o.UserId == userId, cancellationToken);

    public void Add(OrganizerProfile organizerProfile) =>
        dbContext.OrganizerProfiles.Add(organizerProfile);
}
