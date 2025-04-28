using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Application.Interfaces;
using Domain.Entities;
using System.Security.Cryptography;

namespace Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _JwtExpMin;
    private readonly int _RefreshExpDays;

    public JwtService(IConfiguration config)
    {
        _key = config["JWT_KEY"]!;
        _issuer = config["JWT_ISSUER"]!;
        _audience = config["JWT_AUDIENCE"]!;
        _JwtExpMin = Convert.ToInt32(config["Jwt:ExpireMinutes"]!);
        _RefreshExpDays = Convert.ToInt32(config["Jwt:RefreshExpDays"]!);
    }

    public string GenerateToken(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_JwtExpMin),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(Guid userId) {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(_RefreshExpDays),
            UserId = userId
        };
    }
}
