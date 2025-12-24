using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Kohlhaas.Application.DTO.Document;

public record UpdateDocumentDto
{
    [Required]
    public Guid Id { get; set; }
    
    public string? Title { get; set; }
    public DocumentType DocumentType { get; set; }
    public VModelPhase Phase { get; set; }
    public IFormFile? File { get; set; }
    public string? Content { get; set; }
    public List<Guid>? LinkedDocumentIds { get; set; }
}