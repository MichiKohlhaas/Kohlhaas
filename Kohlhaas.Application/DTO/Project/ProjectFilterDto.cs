using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

public record ProjectFilterDto
{
    public VModelPhase? CurrentPhase { get; set; }
    
    public DateTime? StartDateAfter { get; set; }
    public DateTime? StartDateBefore { get; set; }
    
    // pagination stuff
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
    
    [MaxLength(200)]
    public string? SearchText { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}