using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.Document;

// For future reference LINK: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0
public record UploadFileDto
{
    [Required]
    public Guid DocumentId { get; set; }

    [Required] public IFormFile File { get; set; } = null!;
}