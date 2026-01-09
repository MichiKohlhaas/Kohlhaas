using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.ProjectMember;

public record ProjectMemberDetailDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public Guid UserId { get; init; }
    
    public ProjectRole Role { get; init; }
    public DateTime JoinedAt { get; init; }
    public DateTime? LeftAt { get; init; }
    public bool IsOwner { get; init; }
    public bool CanEditRole { get; init; }
    /// <summary>
    /// Admin/PM/Owner should have control over this.
    /// </summary>
    public bool CanLeaveProject { get; init; }
}