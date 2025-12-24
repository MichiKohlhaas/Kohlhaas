namespace Kohlhaas.Application.DTO.RelationshipType;

public record RelationshipTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? ColourCode { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsSystemDefined { get; init; }
}