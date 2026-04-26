using CarWash.Backend.DTOs.Auth;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return new AuthServiceResult<AuthResponse>
            {
                ErrorMessage = "Email already exists."
            };
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Customer"
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return new AuthServiceResult<AuthResponse>
        {
            IsSuccess = true,
            Response = new AuthResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Message = "Registration successful."
            }
        };
    }

    public async Task<AuthServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            return new AuthServiceResult<AuthResponse>
            {
                ErrorMessage = "Invalid email or password."
            };
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return new AuthServiceResult<AuthResponse>
            {
                ErrorMessage = "Invalid email or password."
            };
        }

        var token = _jwtService.GenerateToken(user);

        return new AuthServiceResult<AuthResponse>
        {
            IsSuccess = true,
            Response = new AuthResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                Message = "Login successful."
            }
        };
    }

    public async Task<AuthServiceResult<ProfileResponse>> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return new AuthServiceResult<ProfileResponse>
            {
                IsNotFound = true,
                ErrorMessage = "User not found."
            };
        }

        return new AuthServiceResult<ProfileResponse>
        {
            IsSuccess = true,
            Response = new ProfileResponse
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Phone = user.Phone,
                Message = "Profile fetched successfully."
            }
        };
    }
}
