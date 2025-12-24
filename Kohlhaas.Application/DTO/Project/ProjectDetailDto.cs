using Kohlhaas.Application.DTO.ProjectMember;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Project;

public record ProjectDetailDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; }  = string.Empty;
    public string Code { get; init; }  = string.Empty;
    public VModelPhase CurrentPhase { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? TargetEndDate { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public Guid CreatedById { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    
    /// <summary>
    /// This should include the owner and any/all PMs.
    /// </summary>
    public List<ProjectMemberSummaryDto> Members { get; init; } = [];
    
    // Document metrics?
    /// <summary>
    /// NOTE TO SELF: probably cache these numbers, so I don't have to query the DB each time for a count. 
    /// </summary>
    public Dictionary<DocumentStatus, int> DocumentCountByStatus { get; init; } = [];
    public Dictionary<DocumentType, int> DocumentCountByType { get; init; } = [];
    public int DocumentsOverdue { get; init; }
    
    public bool CanEdit { get; init; }
    public bool CanArchive { get; init; }
    public bool CanAdvancePhase { get; init; }
}