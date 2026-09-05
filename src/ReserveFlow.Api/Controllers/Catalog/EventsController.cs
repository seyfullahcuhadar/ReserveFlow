using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Api.Controllers.Catalog.Dtos;
using ReserveFlow.Api.Extensions;
using ReserveFlow.Application.Catalog.CancelEvent;
using ReserveFlow.Application.Catalog.CreateEvent;
using ReserveFlow.Application.Catalog.PublishEvent;
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

        var eventIdResult = await handler.HandleAsync(command, cancellationToken);
        if (eventIdResult.IsFailure)
        {
            return eventIdResult.ToProblemDetails();
        }

        return CreatedAtAction(
            nameof(Create),
            new CreateEventResponse(eventIdResult.Value));
    }

    [HttpPost("{eventId:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Publish(
        Guid eventId,
        [FromServices] ICommandHandler<PublishEventCommand> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new PublishEventCommand(eventId), cancellationToken);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return NoContent();
    }

    [HttpPost("{eventId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
        Guid eventId,
        [FromServices] ICommandHandler<CancelEventCommand> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CancelEventCommand(eventId), cancellationToken);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return NoContent();
    }
}
