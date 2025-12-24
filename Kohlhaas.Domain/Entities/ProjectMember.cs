using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

/// <summary>
/// Associative entity so we have more control over which users/entities belong to a project.
/// What a user *can* do on a per-project basis.
/// </summary>
public class ProjectMember : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public ProjectRole Role { get; set; } = ProjectRole.None;
    /// <summary>
    /// Ownership flag. Owner is not about operational permissions, just for accountability.
    /// Assuming 1 project = 1 owner.
    /// </summary>
    public bool IsOwner { get; set; }
    public bool IsActive { get; set; } = true;
    
    // For audit purposes
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    
    public bool CanManageProject() => IsOwner || Role >= ProjectRole.Manager;
    public bool HasAccess() => IsActive && (Role > ProjectRole.None || IsOwner);
}
