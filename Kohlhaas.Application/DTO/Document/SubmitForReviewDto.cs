using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// DTO to represent the activity of submitting a document for review and assigning reviewers. 
/// </summary>
public record SubmitForReviewDto
{
    [Required]
    public Guid DocumentId { get; init; }
    [Required]
    public List<Guid> ReviewersIds { get; init; } = [];
    
    public DateTime? DueDate { get; init; }
    public string? Notes { get; init; }
}