using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// For when a document is approved/rejected
/// </summary>
public record ApprovalDecisionDto
{
    [Required]
    public Guid DocumentId { get; set; }
    [Required]
    public ReviewStatus Decision { get; set; }
    [Required]
    [MaxLength(200)]
    public string Comment { get; set; } = string.Empty;
}