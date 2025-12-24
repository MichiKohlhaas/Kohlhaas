using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.RelationshipType;

public record CreateRelationshipTypeDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(300)]
    public string? Description { get; set; }
    [MaxLength(50)]
    public string? Icon { get; set; }
    [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
    public string? ColourCode { get; set; }
    [Range(0, 100)]
    public int DisplayOrder { get; set; }
}