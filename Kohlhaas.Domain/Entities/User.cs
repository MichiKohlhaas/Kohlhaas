using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

/// <summary>
/// What a user *is*
/// </summary>
public class User : EntityBase
{
    public DateTime? LastLoginAt { get; set; }
    public DateTime? PasswordChangeAt { get; set; }
    /*public DateTime LastLockout { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime LastPasswordReset { get; set; }
    public DateTime LastLockoutReset { get; set; }*/
    
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public string Department { get; set; } = string.Empty;
    
    public bool IsReviewer() => Role >= UserRole.Reviewer && IsActive;
    public bool IsProjectManager() => Role >= UserRole.ProjectManager && IsActive;
    public bool IsAdmin() => Role == UserRole.Admin && IsActive;
}