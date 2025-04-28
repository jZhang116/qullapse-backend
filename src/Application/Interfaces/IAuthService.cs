using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto request);
    Task<AuthResponseDto?> LoginAsync(LoginPayloadDto request);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
}
