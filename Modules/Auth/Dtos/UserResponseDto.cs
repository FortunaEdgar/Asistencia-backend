namespace Backend.Modules.Auth.Dtos;

public class UserResponseDto
{
    public Guid Uuid { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}