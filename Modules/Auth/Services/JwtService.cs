using System.Security.Claims;
using System.Text;
using Backend.Modules.Auth.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Modules.Auth.Services;

public class JwtService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public (string Token, DateTime ExpiresAt) GenerateToken(User user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Falta Jwt:Key en los secretos de usuario");
        var minutes = int.Parse(_configuration["Jwt:ExpiresMinutes"] ?? "60");
        var expiresAt = DateTime.UtcNow.AddMinutes(minutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
           {
               new Claim(JwtRegisteredClaimNames.Sub, user.Uuid.ToString()),
               new Claim(JwtRegisteredClaimNames.Email, user.Email),
           }),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256),
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return (token, expiresAt);
    }
}