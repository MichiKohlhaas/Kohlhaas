using Kohlhaas.Application.DTO.User.Supporting;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User;

/// <summary>
/// DTO for details about a specific user.
/// </summary>
public record UserDetailDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; }  = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public bool IsActive { get; init; }
    public UserRole Role { get; init; }
    public string? Department { get; init; }
    
    public List<ProjectMembershipDto> ProjectMemberships { get; init; } = [];
    
    // Computed
    public bool CanEditProfile { get; init; }
    public bool CanEditRole { get; init; }
    public bool CanDeleteProfile { get; init; }
}