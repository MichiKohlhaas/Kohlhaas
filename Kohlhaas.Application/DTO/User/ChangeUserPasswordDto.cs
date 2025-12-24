using System.ComponentModel.DataAnnotations;

namespace Kohlhaas.Application.DTO.User;

public record ChangeUserPasswordDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(128, ErrorMessage = "Password must be between 8 and 128 characters long.")]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^A-Za-z0-9]).{8,}$")]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}