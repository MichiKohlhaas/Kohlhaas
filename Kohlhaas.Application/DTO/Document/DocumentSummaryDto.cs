using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// Fields -> columns for display.
/// </summary>
public record DocumentSummaryDto
{
    public Guid Id { get; init; }
    public string DocumentCode { get; init; } = string.Empty;
    public string DocumentTitle { get; init; } = string.Empty;
    public DocumentType Type { get; init; }
    public VModelPhase Phase { get; init; }
    public DocumentStatus Status { get; init; }
    public int CurrentVersion { get; init; }
    public string AuthorName { get; init; }  = string.Empty;
    public DateTime LastUpdated { get; init; }
    public bool HasRelationships { get; init; }
    public bool HasAssignedReviewer { get; init; }
    public bool IsOverdue { get; init; }
}