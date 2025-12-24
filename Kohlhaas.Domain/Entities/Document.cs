using System.Runtime.CompilerServices;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

public class Document : EntityBase
{
    public Guid ProjectId { get; set; }
    // Document info
    public string Title { get; set; } = string.Empty;
    public string ControlNumber { get; set; }  = string.Empty; // e.g: FS-01
    public string Content { get; set; }  = string.Empty;
    // Document context
    public DocumentType Type { get; set; }
    public VModelPhase Phase { get; set; }
    public DocumentStatus Status { get; set; }
    public int VersionNumber { get; set; }
    
    public Guid? AuthorId => this.CreatedById;

    // File info
    public string? FilePath { get; set; }
    public string? FileHash { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    
    // Submit for review
    public DateTime? ReviewDueDate { get; set; }
    public DateTime? SubmittedForReviewAt { get; set; }
    public Guid? SubmittedById { get; set; }
    public string? SubmissionNotes { get; set; }
    
    public string? UnlockReason { get; set; }
    
    // https://learn.microsoft.com/en-us/ef/core/querying/related-data/lazy
    public virtual ICollection<DocumentReview> Reviews { get; set; } = new List<DocumentReview>();
    public virtual ICollection<DocumentLink> Links { get; set; } = new List<DocumentLink>();

    // BL
    public bool CanBeEditedBy(User user, ProjectMember? member)
    {
        if (IsLocked) return false;
        if (member?.IsActive != true) return false;
        if (member.IsOwner) return true;
        if (member.Role >= ProjectRole.Manager) return true;
        if (AuthorId == user.Id && member.Role > ProjectRole.None) return true;
        return user.Role == UserRole.Admin;
    }
    
    public bool CanBeApprovedBy(User user, ProjectMember? member)
    {
        if (member?.IsActive is not true || member.Role == ProjectRole.None) return false;
        if (Status != DocumentStatus.InReview) return false;
        if (user.Role < UserRole.Reviewer) return false;
        if (member.IsOwner || member.Role == ProjectRole.Manager) return true;
        return member.Role >= ProjectRole.Member;
    }
    public bool IsOverdue => ReviewDueDate.HasValue && 
                             ReviewDueDate < DateTime.Now &&
                             Status == DocumentStatus.InReview;

    private bool IsLocked => Status >= DocumentStatus.Approved;
}