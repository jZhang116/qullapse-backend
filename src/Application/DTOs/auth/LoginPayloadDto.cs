namespace Application.DTOs;

public class LoginPayloadDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
