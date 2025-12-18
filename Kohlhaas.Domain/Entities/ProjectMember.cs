using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

/// <summary>
/// Associative entity so we have more control over which users/entities belong to a project.
/// Focus on what the entity has access to, not what it can do. <see cref="User"/>
/// </summary>
public class ProjectMember : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }

    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }

    public ProjectRole Role { get; set; } = ProjectRole.None;
    
    // For audit purposes
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
    public DateTime? LeftAt { get; set; }
    
    public bool IsActive => IsDeleted is false && LeftAt is null && Role > ProjectRole.None;
    public bool IsOwner => Role == ProjectRole.Owner && IsActive;
    public bool IsManager => Role == ProjectRole.Manager && IsActive;
}