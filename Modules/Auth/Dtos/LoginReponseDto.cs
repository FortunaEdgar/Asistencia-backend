namespace Backend.Modules.Auth.Dtos;

public class LoginResponseDto
{
    public required string Message { get; set; }
    public required UserResponseDto User { get; set; }
}