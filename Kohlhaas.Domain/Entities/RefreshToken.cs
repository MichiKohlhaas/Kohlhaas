namespace Kohlhaas.Domain.Entities;

/// <summary>
/// (JWT) refresh token.
/// </summary>
public class RefreshToken : IEntity
{
    public Guid Id { get; init; }
    /// <summary>
    /// UTC
    /// </summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>
    /// UTC
    /// </summary>
    public DateTime ExpiresAt { get; init; }
    /// <summary>
    /// Would be used for soft delete, but a RefreshToken has no reason to use this.
    /// </summary>
    public bool IsDeleted { get; set; } 
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    // Navigation property
    public User? User { get; init; }
}