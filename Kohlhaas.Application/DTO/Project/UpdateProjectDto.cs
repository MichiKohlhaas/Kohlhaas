using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

public record UpdateProjectDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateTime? StartDate { get; set; }
    [Required]
    public DateTime? TargetEndDate { get; set; }
}