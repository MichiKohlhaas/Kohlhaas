namespace Kohlhaas.Application.DTO.User.Supporting;

public sealed record RefreshTokenDto
{
    public Guid TokenId { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
}