using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// For searching + filters of documents.
/// </summary>
public record DocumentFilterDto
{
    // What can we filter by?
    public Guid? ProjectId { get; set; }
    public DocumentType? Type { get; set; }
    public DocumentStatus? Status { get; set; }
    public VModelPhase? Phase { get; set; }
    
    public DateTime? CreatedFrom { get; set; }
    public DateTime? UpdatedFrom { get; set; }
    
    public bool? HasRelationships { get; set; }
    public bool? HasAssignedReviewer { get; set; }
    public bool? IsArchived { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsLocked { get; set; }
    public bool? IsOverdue { get; set; }
    
    public Guid? AuthorId { get; set; }
    public Guid? ReviewerId { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
    /*public int? CurrentPage { get; set; }
    public int? TotalPages { get; set; }*/
    
    // For searching ControlNumber/document title/content
    public string? SearchText { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public bool SortDescending { get; set; } = false;
}