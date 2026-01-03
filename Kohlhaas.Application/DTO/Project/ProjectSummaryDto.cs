using Kohlhaas.Application.DTO.ProjectMember;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

public record ProjectSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public VModelPhase CurrentPhase { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? TargetEndDate { get; init; }
    public string OwnerName { get; init; }  = string.Empty;
    public int MembersCount { get; init; }
    public int DocumentsCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}