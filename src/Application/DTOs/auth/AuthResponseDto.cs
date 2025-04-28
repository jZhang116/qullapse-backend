using Domain.Entities;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public RefreshToken RefreshToken { get; set; } = default!;
}