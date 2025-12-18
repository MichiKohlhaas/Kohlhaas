namespace Kohlhaas.Domain.Entities;

public interface IEntity
{
    Guid Id { get; } 
    DateTime CreatedAt { get; }
    bool IsDeleted { get; set; }
}