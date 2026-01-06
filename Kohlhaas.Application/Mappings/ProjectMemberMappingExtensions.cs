using Kohlhaas.Application.DTO.ProjectMember;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.Mappings;

public static class ProjectMemberMappingExtensions
{
    extension(ProjectMember source)
    {
        public ProjectMemberDetailDto ToProjectMemberDetailDto(string fullName)
        {
            return new ProjectMemberDetailDto
            {
                Id = source.Id,
                Email = source.Email ?? string.Empty,
                Role = source.Role,
                JoinedAt = source.JoinedAt,
                LeftAt = source.LeftAt,
                ProjectId = source.ProjectId,
                UserId = source.UserId,
                IsOwner = source.IsOwner,
                CanEditRole = source.IsOwner || source.Role >= ProjectRole.Manager,
                CanLeaveProject = source.IsOwner || source.Role >= ProjectRole.Manager,
                FullName = fullName,
            };
        }
        
        public ProjectMemberSummaryDto ToProjectMemberSummaryDto()
        {
            return new ProjectMemberSummaryDto()
            {
                Id = source.Id,
                IsActive = source.IsActive,
                IsOwner = source.IsOwner,
                JoinedAt = source.JoinedAt,
                LeftAt = source.LeftAt,
                Role = source.Role,
                Email =  source.Email,
            };
        }
    }
    
    extension(IEnumerable<ProjectMember> source)
    {
        public IEnumerable<ProjectMemberSummaryDto> ToProjectMemberSummaryDtos()
        {
            return source.Select(ToProjectMemberSummaryDto);
        }
    }
}