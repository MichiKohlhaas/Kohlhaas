using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User;

public record UpdateUserProfileDto
{
    [Required]
    public Guid Id { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    [MaxLength(50)] 
    public string LastName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string Email { get; set; } = string.Empty;
    [MaxLength(100)]
    public string Department { get; set; } =  string.Empty;
}