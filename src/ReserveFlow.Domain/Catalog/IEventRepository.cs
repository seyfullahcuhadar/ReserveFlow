
namespace ReserveFlow.Domain.Catalog;

public interface IEventRepository
{
    void Add(Event @event);
    Task<Event?> GetByIdAsync(Guid eventId, CancellationToken cancellationToken);
}
