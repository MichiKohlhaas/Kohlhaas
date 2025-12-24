using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.Document;

public record DocumentLinkDto
{
    [Required]
    public Guid SourceId { get; set; }
    [Required]
    public Guid TargetId { get; set; }
    [Required]
    public Guid RelationshipTypeId { get; set; }
    [Required]
    public Guid LinkCreatedById { get; set; }
    [Required]
    public DateTime LinkCreatedOn { get; set; }
}