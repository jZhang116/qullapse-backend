using System.Security.Cryptography;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly PasswordHasher<string> _passwordHasher = new();

    public AuthService(IJwtService jwtService, IUserRepository userRepo, IRefreshTokenRepository refreshRepo)
    {
        _jwtService = jwtService;
        _userRepo = userRepo;
        _refreshRepo = refreshRepo;
    }

    public async Task RegisterAsync(RegisterDto request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Email, request.Password)
        };

        await _userRepo.AddAsync(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginPayloadDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email);
        if (user is null)
            return null;

        var verificationResult = _passwordHasher.VerifyHashedPassword(dto.Email, user.PasswordHash, dto.Password);
        if (verificationResult != PasswordVerificationResult.Success)
            return null;

        var accessToken = _jwtService.GenerateToken(user.Id, user.Email);
        var refreshToken = _jwtService.CreateRefreshToken(user.Id);

        await _refreshRepo.AddAsync(refreshToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public RefreshToken GenerateRefreshToken()
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshTokenValue)
    {
        var existingToken = await _refreshRepo.GetByTokenAsync(refreshTokenValue);

        if (existingToken == null || !existingToken.IsActive)
            return null;

        var user = existingToken.User;

        var newAccessToken = _jwtService.GenerateToken(user.Id, user.Email);

        var newRefreshToken = _jwtService.CreateRefreshToken(user.Id);


        await _refreshRepo.AddAsync(newRefreshToken);

        await _refreshRepo.RevokeAsync(existingToken.Token);

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

}
