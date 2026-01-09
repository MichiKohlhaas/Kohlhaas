using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Domain.Entities;

namespace Kohlhaas.Application.Mappings;

public static class ProjectMappingExtensions
{
    extension(Project project)
    {
        public ProjectDetailDto ToProjectDetailDto()
        {
            return new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Code = project.Code,
                CurrentPhase = project.CurrentPhase,
                StartDate = project.StartDate,
                TargetEndDate = project.TargetEndDate,
                CreatedAt = project.CreatedAt,
                CreatedById = project.CreatedById ?? Guid.Empty,
                CreatedByName = project.OwnerName,
                UpdatedAt = project.ModifiedAt,
                Members = [],
                
                IsArchived =  project.IsArchived,
                ArchiveReason = project.ArchiveReason,
                ArchiveNotes = project.ArchiveNotes,
            };
        }

        public ProjectSummaryDto ToProjectSummaryDto()
        {
            return new ProjectSummaryDto()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Code = project.Code,
                CurrentPhase = project.CurrentPhase,
                StartDate = project.StartDate,
                TargetEndDate = project.TargetEndDate,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.ModifiedAt,
                MembersCount = project.Members.Count,
                OwnerName = project.OwnerName,
                DocumentsCount = project.DocumentsCount,
            };
        }
    }

    extension(IEnumerable<Project> projects)
    {
        public IEnumerable<ProjectSummaryDto> ToProjectSummaryDtos()
        {
            return projects.Select(ToProjectSummaryDto);
        }

        public PagedProjectsDto ToPagedProjectsDto(int totalCount, int pageNumber, int pageSize)
        {
            return new PagedProjectsDto()
            {
                Items = projects.ToProjectSummaryDtos().ToList(),
                TotalCount = totalCount,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
    }
}