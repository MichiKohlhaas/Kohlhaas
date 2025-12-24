using Kohlhaas.Application.DTO.Document.Supporting;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

public record DocumentDetailDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }       
    public string ControlNumber { get; init; } = string.Empty;
    
    public string Title { get; init; } = string.Empty;
    public DocumentType DocumentType { get; init; }
    public DocumentStatus Status { get; init; }
    public VModelPhase Phase { get; init; }
    public string? Content { get; init; }
    public int VersionNumber  { get; init; }
    
    public string? FilePath { get; set; }
    public string? FileHash { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }

    public List<Guid> ReviewerIds { get; init; } = [];
    public List<string> ReviewerNames { get; init; } = [];
    public DateTime? ReviewDueDate { get; init; }
    
    public Guid AuthorId { get; init; }
    public string AuthorName { get; init; } =  string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public Guid UpdatedById { get; init; }
    public string? UpdatedBy { get; init; }
    
    public List<LinkedDocumentDto> LinkedDocuments { get; init; } = [];
    public ApprovalHistoryDto? LatestApproval { get; init; }
    
    public bool CanEdit { get; init; }
    public bool CanDelete { get; init; }
    public bool CanView { get; init; }
    public bool CanSubmitForReview { get; init; }
    public bool CanReject  { get; init; }
    public bool CanApprove  { get; init; }
    public bool IsLocked  { get; init; }
    public bool IsOverdue  { get; init; }
}