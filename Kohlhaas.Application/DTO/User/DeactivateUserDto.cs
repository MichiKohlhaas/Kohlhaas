using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.User;

public record DeactivateUserDto
{
    [Required]
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Should include a brief justification.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;
}