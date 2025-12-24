using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

public record CreateProjectDto
{
    [Required]
    public Guid OwnerId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    /// <summary>
    /// Might consider making this a calculated field via BL on the entity.
    /// </summary>
    [Required]
    [MaxLength(8)]
    public string Code { get; set; } = string.Empty;
    

    /// <summary>
    /// Defaults to Requirements, but can change if transferring project case.
    /// </summary>
    [Required] public VModelPhase CurrentPhase { get; set; } = VModelPhase.UserRequirements;
    public DateTime? StartDate { get; set; }
    public DateTime? TargetEndDate { get; set; }
    
}