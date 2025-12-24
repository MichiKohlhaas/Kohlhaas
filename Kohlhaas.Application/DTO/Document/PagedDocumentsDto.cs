using Kohlhaas.Application.DTO.Common;

namespace Kohlhaas.Application.DTO.Document;

/// <summary>
/// Pagination metadata.
/// </summary>
public record PagedDocumentsDto : PagedResult<DocumentSummaryDto>;