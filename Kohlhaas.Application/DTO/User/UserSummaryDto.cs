using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User;

/// <summary>
/// For list purposes.
/// </summary>
public record UserSummaryDto
{
    public Guid Id { get; init; }
    /// <summary>
    /// Possible violation of user privacy. Can remove later if needed.
    /// </summary>
    public string? Email { get; init; }
    /// <summary>
    /// Simple summary doesn't need first and last name if we already have full.
    /// </summary>
    public string FullName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public bool IsActive { get; init; }
    public string? Department { get; init; }
    public DateTime? LastLoginAt { get; init; }
}