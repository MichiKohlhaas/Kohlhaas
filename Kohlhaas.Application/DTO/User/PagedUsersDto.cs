using Kohlhaas.Application.DTO.Common;
using Kohlhaas.Application.DTO.Document;

namespace Kohlhaas.Application.DTO.User;

/// <summary>
/// Pagination metadata
/// </summary>
public record PagedUsersDto : PagedResult<UserSummaryDto>;