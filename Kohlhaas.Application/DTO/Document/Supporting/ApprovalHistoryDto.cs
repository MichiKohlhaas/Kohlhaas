using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document.Supporting;

/// <summary>
/// Latest approval. Used in <see cref="DocumentDetailDto"/>
/// </summary>
/// <param name="ReviewerId"></param>
/// <param name="ReviewerName"></param>
/// <param name="ReviewedAt"></param>
/// <param name="Decision"></param>
/// <param name="Comment"></param>
public record ApprovalHistoryDto(
    Guid ReviewerId,
    string ReviewerName,
    DateTime ReviewedAt,
    ReviewStatus Decision,
    string? Comment
);