using Kohlhaas.Application.DTO.Document.Supporting;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// The document that is under reviewed. Contains all relevant data.
/// </summary>
public record DocumentForReviewDto
{
    public Guid Id { get; init; }
    public ReviewStatus Status { get; init; }
    public string ControlNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DocumentType DocumentType { get; init; }
    public VModelPhase Phase { get; init; }
    public string? Content { get; init; }
    public int VersionNumber  { get; init; }
    
    public string? FileName { get; init; }
    // Don't think I'll need the ID, just the name of the person who submitted for review.
    public string SubmittedByName { get; init; } =  string.Empty;
    public DateTime SubmittedAt { get; init; }
    public DateTime DueDate { get; init; }
    public bool IsOverdue { get; init; }
    public string? SubmissionNotes {get; init; }

    // From previous reviews if rejected. Adds context.
    public List<ReviewCommentDto> PreviousComments { get; init; } = [];
    // Other reviewers' decisions, if any.
    public List<ReviewerDecisionDto> OtherReviewers { get; init; } = [];
    // See doc's edit history.
    public List<DocumentVersionDto> VersionHistory { get; init; } = [];
    // Maybe the reviewer can edit the doc immediately.
    public bool CanEdit { get; init; }
}