using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.RelationshipType;

public record UpdateRelationshipTypeDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ColorCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}