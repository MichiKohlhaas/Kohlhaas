using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User;

/// <summary>
/// Separate DTO for the admin to update a user's role.
/// </summary>
public record UpdateUserRoleDto
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public UserRole NewRole { get; set; }
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;
}