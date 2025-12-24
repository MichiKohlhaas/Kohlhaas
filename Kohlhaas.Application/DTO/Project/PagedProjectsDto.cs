using Kohlhaas.Application.DTO.Common;

namespace Kohlhaas.Application.DTO.Project;

public record PagedProjectsDto : PagedResult<ProjectSummaryDto>;