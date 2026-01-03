using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

public class Project : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string Code { get; set; }  = string.Empty;
    public VModelPhase CurrentPhase { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetEndDate { get; set; }
    public bool IsArchived { get; set; }
    public bool IsReadOnly => IsArchived;
    public ArchiveReason? ArchiveReason { get; set; }
    public string? ArchiveNotes { get; set; }

    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public int DocumentsCount { get; set; }
    
    public virtual ICollection<ProjectMember> Members { get; set; } = [];
    
    public bool CanBeArchivedBy(ProjectMember member) => member.IsOwner;
    public bool CanAdvancePhase(ProjectMember member) => member.IsOwner || member.Role >= ProjectRole.Manager;
}