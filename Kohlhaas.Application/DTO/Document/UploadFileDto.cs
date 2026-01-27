using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document;

// For future reference LINK: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0
public record UploadFileDto
{
    [Required]
    public Guid DocumentId { get; set; }

    [Required]
    public string FileName { get; set; } = string.Empty;
    [Required]
    public long FileSize { get; set; }
    [Required]
    public string? ContentType { get; set; }
    
    //[Required] public IFormFile File { get; set; } = null!;
}