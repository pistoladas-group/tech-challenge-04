using System.Net;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechNews.Auth.Api.Configurations;
using TechNews.Auth.Api.Data;
using TechNews.Auth.Api.Models;
using TechNews.Common.Library.Models;
using TechNews.Auth.Api.Services.KeyRetrievers;

namespace TechNews.Auth.Api.Controllers;

[Route("api/auth/user")]
public class UserController : ControllerBase
{
    private IBus _bus;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ICryptographicKeyRetriever _cryptographicKeyRetriever;

    public UserController(UserManager<User> userManager, SignInManager<User> signInManager, ICryptographicKeyRetriever cryptographicKeyRetriever, IBus bus)
    {
        _bus = bus;
        _userManager = userManager;
        _signInManager = signInManager;
        _cryptographicKeyRetriever = cryptographicKeyRetriever;
    }

    /// <summary>
    /// Creates a new User
    /// </summary>
    /// <param name="user">The user to be registered</param>
    /// <response code="201">Returns the created resource endpoint in response header</response>
    /// <response code="400">There is a problem with the request</response>
    /// <response code="500">There was an internal problem</response>
    [HttpPost("")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterUserRequestModel user)
    {
        var id = user.Id ?? Guid.NewGuid();

        var existingUser = await _userManager.FindByIdAsync(id.ToString());

        if (existingUser is not null)
        {
            return BadRequest(new ApiResponse(error: new ErrorResponse("invalid_request", "UserAlreadyExists", "User already exists")));
        }

        var createUserResult = await _userManager.CreateAsync(new User(id, user.Email, user.UserName), user.Password);

        if (!createUserResult.Succeeded)
        {
            return BadRequest(new ApiResponse(errors: createUserResult.Errors.ToList().ConvertAll(x => new ErrorResponse("invalid_request", x.Code, x.Description))));
        }

        var registeredUserResult = await _userManager.FindByEmailAsync(user.Email);

        if (registeredUserResult?.Email is null)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse(error: new ErrorResponse("server_error", "InternalError", "There was an unexpected error with the application. Please contact support!")));
        }

        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(registeredUserResult);
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{EnvironmentVariables.BrokerQueueName}"));
        var brokerMessage = new ConfirmEmailBrokerMessage(email: registeredUserResult.Email, token: emailToken);
        
        await endpoint.Send(brokerMessage);

        return CreatedAtAction(nameof(GetUser), new { userId = id }, new ApiResponse());
    }

    /// <summary>
    /// Get the user details
    /// </summary>
    /// <param name="userId">The user id to be searched</param>
    /// <response code="200">Returns the resource data</response>
    /// <response code="400">There is a problem with the request</response>
    /// <response code="404">There is no resource with the given id</response>
    /// <response code="500">There was an internal problem</response>
    [HttpGet("{userId:guid}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUser([FromRoute] Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(new ApiResponse(error: new ErrorResponse("invalid_request", "InvalidUser", "The userId is not valid")));
        }

        var getUserResult = await _userManager.FindByIdAsync(userId.ToString());

        if (getUserResult is null)
        {
            return NotFound(new ApiResponse(error: new ErrorResponse("invalid_request", "UserNotFound", "The user was not found")));
        }

        var responseModel = new GetUserResponseModel
        {
            Id = getUserResult.Id,
            UserName = getUserResult.UserName,
            Email = getUserResult.Email,
        };

        return Ok(new ApiResponse(data: responseModel));
    }
}