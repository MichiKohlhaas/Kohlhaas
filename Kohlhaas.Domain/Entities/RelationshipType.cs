namespace Kohlhaas.Domain.Entities;

/// <summary>
/// Made an entity because we will have both system and user-defined relationship types for document links.
/// Not a VO b/c we should be able to edit its data.
/// </summary>
public class RelationshipType : EntityBase
{
    public string Name { get; set; } =  string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ColourCode { get; set; }
    
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    
    public bool IsSystemDefined { get; set; }

    public virtual ICollection<DocumentLink> Links { get; init; } = [];
    
    // Default relationships created by system can't be deleted.
    public bool CanBeDeleted() => IsSystemDefined == false;
}