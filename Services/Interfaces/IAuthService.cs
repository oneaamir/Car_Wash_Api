using CarWash.Backend.DTOs.Auth;

namespace CarWash.Backend.Services.Interfaces;

public interface IAuthService
{
    Task<AuthServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);

    Task<AuthServiceResult<AuthResponse>> LoginAsync(LoginRequest request);

    Task<AuthServiceResult<ProfileResponse>> GetProfileAsync(int userId);
}
