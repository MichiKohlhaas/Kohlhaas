using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User;

public record RegisterUserDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Defaults to the least privilege.
    /// </summary>
    [Required]
    public UserRole Role { get; set; } =  UserRole.User;
    
    [Required]
    [MaxLength(128, ErrorMessage = "Password must be between 8 and 128 characters long.")]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^A-Za-z0-9]).{8,}$")]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; }  = string.Empty;

    [MaxLength(50)]
    public string? Department { get; set; }
}