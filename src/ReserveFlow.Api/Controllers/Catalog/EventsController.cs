using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Api.Controllers.Catalog.Dtos;
using ReserveFlow.Application.Catalog.CreateEvent;
using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Api.Controllers.Catalog;

[ApiController]
[Route("api/v1/events")]
public sealed class EventsController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateEventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateEventResponse>> Create(
        [FromBody] CreateEventRequest request,
        [FromServices] ICommandHandler<CreateEventCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand(
            request.OrganizerId,
            request.VenueId,
            request.Title,
            request.Description,
            request.StartAtUtc,
            request.EndAtUtc,
            request.TicketTypes
                .Select(t => new CreateEventTicketTypeDto(
                    t.Name,
                    t.PriceAmount,
                    t.Currency,
                    t.Quota,
                    t.SalesStartAtUtc,
                    t.SalesEndAtUtc))
                .ToList());

        var eventId = await handler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(
            nameof(Create),
            new CreateEventResponse(eventId));
    }
}
