using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User;


public record UserFilterDto
{
    public UserRole? Role { get; set; }
    public bool? IsActive { get; set; }
    [MaxLength(100)]
    public string? Department { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
    
    [MaxLength(100)]
    public string? SearchText { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}