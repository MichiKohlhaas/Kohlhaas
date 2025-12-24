using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.DTO.User.Supporting;

public record ProjectMembershipDto(
    Guid ProjectId,
    string ProjectName,
    string ProjectCode,
    ProjectRole ProjectRole,
    bool IsOwner
);