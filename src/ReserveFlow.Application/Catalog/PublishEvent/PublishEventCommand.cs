using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Application.Catalog.PublishEvent;

public sealed record PublishEventCommand(Guid EventId) : ICommand;