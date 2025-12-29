namespace Kohlhaas.Application.DTO.User;

public record UserActivityReportDto
{
    public DateTime? LastLoginAt { get; init; }
    public DateTime? PasswordChangeAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    //public DateTime? LastLockout { get; init; }
    //public DateTime? LastActivity { get; init; }
    //public DateTime? LastPasswordReset { get; init; }
    //public DateTime? LastLockoutReset { get; init; }
}