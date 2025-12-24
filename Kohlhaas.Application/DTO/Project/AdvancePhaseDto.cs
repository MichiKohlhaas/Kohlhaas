using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

public record AdvancePhaseDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public VModelPhase TargetPhase { get; set; }
    [MaxLength(500)]
    public string? Reason { get; set; }
}