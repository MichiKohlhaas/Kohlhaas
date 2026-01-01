namespace Kohlhaas.Domain.Entities;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; set; }

    public DateTime? ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public Guid? CreatedById { get; set; }
    public Guid? ModifiedById { get; set; }
    
}