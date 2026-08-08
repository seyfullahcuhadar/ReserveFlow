namespace ReserveFlow.Domain.Catalog;

public interface IEventRepository
{
    void Add(Event @event);
}
