using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

public record CreateDocumentDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public Guid ProjectId { get; set; }
    [Required]
    public DocumentType Type { get; set; }
    
    public string? Content { get; set; }
    public VModelPhase Phase { get; set; }
    public List<Guid>? LinkedDocumentIds { get; set; }
}