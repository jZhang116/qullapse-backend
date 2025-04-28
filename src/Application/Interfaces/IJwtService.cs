using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string email);
    RefreshToken CreateRefreshToken(Guid userId);
}
