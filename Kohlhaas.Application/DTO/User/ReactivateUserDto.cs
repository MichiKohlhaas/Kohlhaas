using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.User;

public record ReactivateUserDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;
}