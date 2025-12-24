namespace Kohlhaas.Application.DTO.Document.Supporting;

/// <summary>
/// Helper record for <see cref="DocumentForReviewDto"/>. List of to have comment history
/// </summary>
/// <param name="ReviewerId"></param>
/// <param name="ReviewerName"></param>
/// <param name="Comment"></param>
/// <param name="ReviewDate"></param>
public record ReviewCommentDto(
    Guid ReviewerId,
    string ReviewerName,
    string Comment, 
    DateTime ReviewDate
);