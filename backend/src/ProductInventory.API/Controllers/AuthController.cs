using Microsoft.AspNetCore.Mvc;
using ProductInventory.Application.DTOs.Auth;
using ProductInventory.Application.Interfaces;

namespace ProductInventory.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < 6)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid registration details",
                Detail = "Username is required and password must contain at least 6 characters.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var response = await authService.RegisterAsync(
            request,
            cancellationToken);

        return response is null
            ? Conflict(new ProblemDetails
            {
                Title = "Username already exists",
                Status = StatusCodes.Status409Conflict
            })
            : Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(
            request,
            cancellationToken);

        return response is null
            ? Unauthorized(new ProblemDetails
            {
                Title = "Invalid username or password",
                Status = StatusCodes.Status401Unauthorized
            })
            : Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var response = await authService.RefreshAsync(
            request,
            cancellationToken);

        return response is null
            ? Unauthorized(new ProblemDetails
            {
                Title = "Invalid or expired refresh token",
                Status = StatusCodes.Status401Unauthorized
            })
            : Ok(response);
    }
}
