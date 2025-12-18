namespace Kohlhaas.Domain.Entities;

/// <summary>
/// Exists as a separate entity because documents can have many links.
/// </summary>
public class DocumentLink : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; }
    
    public Guid SourceId { get; set; }
    public Guid TargetId { get; set; }
    public Guid RelationshipTypeId { get; set; }
    
    // For audit purposes
    public Guid CreatedByUserId { get; set; }
}