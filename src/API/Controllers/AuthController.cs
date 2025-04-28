using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        await _authService.RegisterAsync(dto);
        return StatusCode(201);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginPayloadDto dto)
    {
        var authResult = await _authService.LoginAsync(dto);

        if (authResult == null)
            return Unauthorized();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict ,
            Expires = authResult.RefreshToken.Expires
        };

        Response.Cookies.Append("refreshToken", authResult.RefreshToken.Token, cookieOptions);

        return Ok(new
        {
            accessToken = authResult.AccessToken
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var authResult = await _authService.RefreshTokenAsync(refreshToken);

        if (authResult == null)
            return Unauthorized();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = authResult.RefreshToken.Expires
        };

        Response.Cookies.Append("refreshToken", authResult.RefreshToken.Token, cookieOptions);

        return Ok(new
        {
            accessToken = authResult.AccessToken
        });
    }

    [HttpGet]
    [Route("test")]
    public IActionResult Test()
    {
        return Ok();
    }
}
