using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Api.Controllers.Catalog;
using ReserveFlow.Application.Catalog.CreateOrganizerProfile;
using ReserveFlow.Application.Messaging;

namespace ReserveFlow.Api.Controllers;

[ApiController]
[Route("api/v1/organizers")]
public sealed class OrganizersController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrganizerProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateOrganizerProfileResponse>> Create(
        [FromBody] CreateOrganizerProfileRequest request,
        [FromServices] ICommandHandler<CreateOrganizerProfileCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizerProfileCommand(request.UserId, request.DisplayName, request.Bio);
        var organizerId = await handler.HandleAsync(command, cancellationToken);

        return CreatedAtAction(
            nameof(Create),
            new CreateOrganizerProfileResponse(organizerId));
    }
}
