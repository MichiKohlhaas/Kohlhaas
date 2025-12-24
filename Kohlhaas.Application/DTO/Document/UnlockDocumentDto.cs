using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// Document.Status = Locked (after approval). CanEdit -> False.
/// Unlock always changes status to draft.
/// </summary>
public record UnlockDocumentDto
{
    [Required]
    public Guid DocumentId { get; set; }
    [Required]
    [MaxLength(200)]
    public string Reason { get; set; } = string.Empty;
}