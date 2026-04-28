using AuthService.Application.Abstractions;
using AuthService.Application.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authApplicationService;

    public AuthController(IAuthApplicationService authApplicationService)
    {
        _authApplicationService = authApplicationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authApplicationService.RegisterAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authApplicationService.LoginAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authApplicationService.RefreshTokenAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Value);
    }
}