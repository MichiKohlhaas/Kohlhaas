using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.ProjectMember;

public record RemoveProjectMemberDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public Guid MemberId { get; set; }
    [MaxLength(500)]
    public string? Comment { get; set; }
}