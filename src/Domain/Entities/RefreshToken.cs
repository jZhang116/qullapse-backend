namespace Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = default!;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Expires { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsActive => !IsExpired;

    public Guid UserId { get; set; }
    public User User { get; set; } = default!;
}