using Backend.Modules.Auth.Controllers;
using Backend.Modules.Auth.Repositories;
using Backend.Modules.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend.Modules.Auth;

public static class AddModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<AuthService>();
        services.AddScoped<AuthRepository>();
        services.AddScoped<JwtService>();

        var key = configuration["Jwt:Key"]
          ?? throw new InvalidOperationException("Falta Jwt:Key en los secretos de usuario");

        services
          .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options => {
              options.MapInboundClaims = false;
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = configuration["Jwt:Issuer"],
                  ValidAudience = configuration["Jwt:Audience"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                  ClockSkew = TimeSpan.Zero,
              };
              options.Events = new JwtBearerEvents
              {
                  OnMessageReceived = context => {
                      context.Token = context.Request.Cookies[AuthController.CookieName];
                      return Task.CompletedTask;
                  },
              };
          });

        services.AddAuthorization();
        return services;
    }
}