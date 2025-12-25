namespace Kohlhaas.Domain.Entities;

public interface IEntity
{
    Guid Id { get; init; } 
    DateTime CreatedAt { get; init; }
    bool IsDeleted { get; set; }
}