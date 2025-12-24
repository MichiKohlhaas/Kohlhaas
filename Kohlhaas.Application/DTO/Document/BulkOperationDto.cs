using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// To enable better UX of bulk approving or obsoleting documents.
/// </summary>
public record BulkOperationDto
{
    [Required]
    public List<Guid> DocumentIds { get; set; } = [];
    
    [Required]
    public DocumentStatus TargetStatus { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } =  string.Empty;
}