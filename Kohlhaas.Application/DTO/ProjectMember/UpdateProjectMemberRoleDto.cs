using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.ProjectMember;

public record UpdateProjectMemberRoleDto
{
    [Required]
    public Guid MemberId { get; set; }
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public ProjectRole Role { get; set; }
}