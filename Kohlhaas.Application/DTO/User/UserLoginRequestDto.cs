using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.User;

/// <summary>
/// For encapsulating the login variables.
/// </summary>
public record UserLoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}