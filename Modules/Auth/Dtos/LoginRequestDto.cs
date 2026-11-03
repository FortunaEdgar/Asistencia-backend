using System.ComponentModel.DataAnnotations;

namespace Backend.Modules.Auth.Dtos;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El Email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El Email no es válido.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    public required string Password { get; set; }
}
