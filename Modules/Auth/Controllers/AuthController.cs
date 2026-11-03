using System.Security.Claims;
using Backend.Modules.Auth.Dtos;
using Backend.Modules.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
namespace Backend.Modules.Auth.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class AuthController(AuthService AuthService) : ControllerBase
{
    private readonly AuthService _authService = AuthService;
    public const string CookieName = "access_token";

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto registerRequestDto)
    {
        var user = await _authService.RegisterAsync(registerRequestDto);
        if (user is null) return Conflict(new { message = "El email ya esta registrado" });
        return Created("/api/v1/auth/me", user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
    {
        var result = await _authService.LoginAsync(loginRequestDto);
        if (result is null) return Unauthorized(new { message = "Credenciales inválidas" });

        Response.Cookies.Append(CookieName, result.Token, BuildCookieOptions(result.ExpiresAt));
        return Ok(new LoginResponseDto { Message = "Inicio de sesion exitoso", User = result.User });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(sub, out var uuid)) return Unauthorized();

        var user = await _authService.GetByUuidAsync(uuid);
        return user is null ? Unauthorized() : Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(CookieName, BuildCookieOptions(null));
        return Ok(new { message = "Sesion cerrada" });
    }

    private CookieOptions BuildCookieOptions(DateTime? expiresAt) => new()
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        Expires = expiresAt,
    };
}