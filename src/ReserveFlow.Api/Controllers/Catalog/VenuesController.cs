using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Api.Controllers.Catalog.Dtos;
using ReserveFlow.Api.Extensions;
using ReserveFlow.Application.Catalog.CreateVenue;
using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Api.Controllers.Catalog;

[ApiController]
[Route("api/v1/venues")]
public sealed class VenuesController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateVenueResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateVenueResponse>> Create(
        [FromBody] CreateVenueRequest request,
        [FromServices] ICommandHandler<CreateVenueCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateVenueCommand(
            request.Name,
            request.Street,
            request.City,
            request.Country,
            request.PostalCode,
            request.Capacity,
            request.TimeZone);

        var venueIdResult = await handler.HandleAsync(command, cancellationToken);
        if (venueIdResult.IsFailure)
        {
            return venueIdResult.ToProblemDetails();
        }

        return CreatedAtAction(
            nameof(Create),
            new CreateVenueResponse(venueIdResult.Value));
    }
}
