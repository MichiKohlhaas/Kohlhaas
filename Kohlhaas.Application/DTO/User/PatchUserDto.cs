using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.User;

/// <summary>
/// DTO for JSON PATCH operations, partial update.
/// </summary>
public record PatchUserDto
{
    [MaxLength(50)]
    public string? FirstName { get; set; }
    [MaxLength(50)]
    public string? LastName { get; set; }
    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }
    [MaxLength(100)]
    public string? Department { get; set; }
}