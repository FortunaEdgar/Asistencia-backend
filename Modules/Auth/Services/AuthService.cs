using Backend.Modules.Auth.Dtos;
using Backend.Modules.Auth.Entities;
using Backend.Modules.Auth.Repositories;
using Backend.Modules.Auth.Services;

namespace Backend.Modules.Auth.Services;

public record LoginResult(UserResponseDto User, string Token, DateTime ExpiresAt);

public class AuthService(AuthRepository authRepository, JwtService jwtService)
{
    private readonly AuthRepository _authRepository = authRepository;
    private readonly JwtService _jwtService = jwtService;

    public async Task<UserResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        var email = dto.Email.Trim().ToLower();
        if (await _authRepository.EmailExistsAsync(email)) return null;

        var user = new User
        {
            Uuid = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };
        await _authRepository.CreateAsync(user);
        return ToDto(user);
    }

    public async Task<LoginResult?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _authRepository.FindByEmailAsync(dto.Email.Trim().ToLower());
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)) return null;

        var (token, expiresAt) = _jwtService.GenerateToken(user);
        return new LoginResult(ToDto(user), token, expiresAt);
    }

    public async Task<UserResponseDto?> GetByUuidAsync(Guid uuid)
    {
        var user = await _authRepository.FindByUuidAsync(uuid);
        return user is null ? null : ToDto(user);
    }

    private static UserResponseDto ToDto(User user) =>
      new() { Uuid = user.Uuid, Name = user.Name, Email = user.Email };
}