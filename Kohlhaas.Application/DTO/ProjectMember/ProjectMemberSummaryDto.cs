using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.ProjectMember;

public record ProjectMemberSummaryDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public ProjectRole Role { get; init; }
    public bool IsActive { get; init; }
    public bool IsOwner { get; init; }
    public DateTime JoinedAt { get; init; }
    public DateTime? LeftAt { get; init; }
}