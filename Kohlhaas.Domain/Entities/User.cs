using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

/// <summary>
/// Focus on what the user can do, not what the user can access <see cref="ProjectMember"/>
/// </summary>
public class User : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    
    public DateTime LastLoginAt { get; set; }
    public DateTime PasswordChangeAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
    /*public DateTime LastLockout { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime LastPasswordReset { get; set; }
    public DateTime LastLockoutReset { get; set; }*/
    
    //public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Viewer;
    public bool IsActive { get; set; }
    public string Department { get; set; } = string.Empty;
    
    public bool CanCreateDocument() => Role >= UserRole.Contributor && IsActive;
    public bool CanCreateProject() => Role >= UserRole.Admin && IsActive;
    public bool CanViewDocument() => Role == UserRole.Viewer && IsActive;
    public bool CanEditDocument() => Role == UserRole.Contributor && IsActive;
    public bool CanReviewDocument() => Role == UserRole.Reviewer && IsActive;
    public bool CanDeleteDocument() => Role == UserRole.Admin && IsActive;
}