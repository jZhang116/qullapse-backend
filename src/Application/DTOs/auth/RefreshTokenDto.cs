public class RefreshTokenDto
{
    public string Token { get; set; } = default!;
    public DateTime Expires { get; set; } = default!;
}