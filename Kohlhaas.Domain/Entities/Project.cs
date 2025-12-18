using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

public class Project : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string Code { get; set; }  = string.Empty;
    public VModelPhase CurrentPhase { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsArchived { get; set; }
    public virtual ICollection<ProjectMember> Members { get; set; } = [];
    
}