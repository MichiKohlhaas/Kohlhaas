using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.Document.Supporting;

/// <summary>
/// 
/// </summary>
public record DocumentVersionDto(
    int VersionNumber,
    string UpdatedByName,
    ReviewStatus PreviousStatus,
    DateTime UpdatedAt
);