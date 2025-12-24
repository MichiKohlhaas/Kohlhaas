using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.ProjectMember;

/// <summary>
/// For the use case assigning user to project.
/// </summary>
public record CreateProjectMemberDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public ProjectRole Role { get; set; } = ProjectRole.None;
}