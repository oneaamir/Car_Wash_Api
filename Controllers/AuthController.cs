using CarWash.Backend.Data;
using CarWash.Backend.DTOs.Auth;
using CarWash.Backend.Models;
using CarWash.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var emailExists = await _context.Users
            .AnyAsync(user => user.Email == request.Email);

        if (emailExists)
        {
            return BadRequest("Email already exists.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Customer"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Message = "Registration successful."
        };

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(user => user.Email == request.Email);

        if (user == null)
        {
            return BadRequest("Invalid email or password.");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return BadRequest("Invalid email or password.");
        }

        var token = _jwtService.GenerateToken(user);

        var response = new AuthResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            Message = "Login successful."
        };

        return Ok(response);
    }
}
