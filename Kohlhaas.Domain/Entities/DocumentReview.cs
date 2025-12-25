using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Domain.Entities;

/// <summary>
/// A document can have many reviewers -> separate entity to track each review.
/// </summary>
public class DocumentReview : IEntity
{
    public Guid Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; set; }
    
    public Guid DocumentId { get; set; }
    public Guid ReviewerId { get; set; }
    
    public ReviewStatus? Decision { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? Comment { get; set; }
}