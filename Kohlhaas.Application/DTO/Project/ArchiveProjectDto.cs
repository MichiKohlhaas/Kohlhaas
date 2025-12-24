using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

/// <summary>
/// Project is concluded. No active development. Read-only view.
/// </summary>
public record ArchiveProjectDto
{
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public string? Notes { get; set; }
    /// <summary>
    /// Something like "completed", "obsolete", or "cancelled"
    /// </summary>
    [Required]
    public ArchiveReason ArchiveReason { get; set; }
}