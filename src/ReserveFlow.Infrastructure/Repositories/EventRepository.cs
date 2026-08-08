using ReserveFlow.Domain.Catalog;
using Event = ReserveFlow.Domain.Catalog.Event;

namespace ReserveFlow.Infrastructure.Repositories;

public sealed class EventRepository(ApplicationDbContext dbContext) : IEventRepository
{
    public void Add(Event @event) =>
        dbContext.Events.Add(@event);
}
