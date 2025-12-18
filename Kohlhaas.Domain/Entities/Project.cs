using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

public class Project : EntityBase
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string ProjectCode { get; set; }  = string.Empty;
    public VModelPhase CurrentPhase { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public virtual ICollection<ProjectMember> Members { get; set; } = [];
    
}