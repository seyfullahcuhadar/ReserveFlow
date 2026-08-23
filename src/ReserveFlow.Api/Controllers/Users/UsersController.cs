using Microsoft.AspNetCore.Mvc;
using ReserveFlow.Api.Controllers.Users.Dtos;
using ReserveFlow.Application.Messaging;
using ReserveFlow.Application.Users.LoginUser;
using ReserveFlow.Application.Users.RegisterUser;
using Wolverine;

namespace ReserveFlow.Api.Controllers.Users;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IMessageBus _messageBus;

    public UsersController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterUserResponse>> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email, request.Password);
        var userId =await  _messageBus.InvokeAsync<Guid>(command);

        return CreatedAtAction(
            nameof(Register),
            new RegisterUserResponse(userId));
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
        var token = await loginUserCommandHandler.HandleAsync(command, cancellationToken);

        return Ok(new LoginUserResponse(token));
    }
}
