using System.Security.Claims;
using CarWash.Backend.DTOs.Auth;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileResponse>> GetProfile()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var result = await _authService.GetProfileAsync(userId);

        if (result.IsNotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Response);
    }
}
