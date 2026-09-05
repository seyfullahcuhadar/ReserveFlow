using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Api.Controllers.Users.Dtos;
using ReserveFlow.Api.Extensions;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Users.LoginUser;
using ReserveFlow.Application.Users.RegisterUser;

namespace ReserveFlow.Api.Controllers.Users;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterUserResponse>> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] ICommandHandler<RegisterUserCommand, Guid> handler,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var result = await handler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return CreatedAtAction(
            nameof(Register),
            new RegisterUserResponse(result.Value));
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginUserResponse>> Login(
        [FromBody] LoginUserRequest request,
        [FromServices] ICommandHandler<LoginUserCommand, string> loginUserCommandHandler,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await loginUserCommandHandler.HandleAsync(command, cancellationToken);
        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return Ok(new LoginUserResponse(result.Value));
    }
}
