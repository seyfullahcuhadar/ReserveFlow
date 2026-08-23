using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Application.Catalog.CancelEvent;

public sealed record CancelEventCommand(Guid EventId) : ICommand;
