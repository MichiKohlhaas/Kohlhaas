using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document.Supporting;

/// <summary>
/// Helper record for <see cref="DocumentForReviewDto"/>. Represents the decision for a review.
/// </summary>
/// <param name="ReviewerId"></param>
/// <param name="ReviewerName"></param>
/// <param name="ReviewerStatus"></param>
/// <param name="ReviewDate"></param>
public record ReviewerDecisionDto(
    Guid ReviewerId,
    string ReviewerName,
    ReviewStatus ReviewerStatus,
    DateTime ReviewDate
);