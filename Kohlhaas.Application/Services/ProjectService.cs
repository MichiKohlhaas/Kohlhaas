using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Application.DTO.ProjectMember;
using Kohlhaas.Application.Interfaces.Project;
using Kohlhaas.Application.Interfaces.Token;
using Kohlhaas.Application.Mappings;
using Kohlhaas.Common.Result;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;
using Kohlhaas.Domain.Interfaces;

namespace Kohlhaas.Application.Services;

public sealed class ProjectService(IUnitOfWork unitOfWork, ITokenService tokenService) : IProjectService
{
    public Task<Result<ProjectMemberDetailDto>> AssignToProjectAsync(CreateProjectMemberDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProjectMemberDetailDto>> UpdateProjectRoleAsync(UpdateProjectMemberRoleDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RemoveFromProjectAsync(Guid projectId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProjectMemberDetailDto>> GetProjectMemberAsync(Guid projectId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<ProjectMemberSummaryDto>>> GetProjectMembersAsync(Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<ProjectMemberDetailDto>>> GetUserProjectsAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> IsProjectMemberAsync(Guid projectId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProjectDetailDto>> TransferOwnershipAsync(Guid projectId, Guid newOwnerId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ProjectDetailDto>> CreateProjectAsync(Guid creatorId, CreateProjectDto dto)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var creatorUser = userRepo.GetById(creatorId);
        var ownerUser = userRepo.GetById(dto.OwnerId);
        
        await Task.WhenAll(creatorUser, ownerUser);

        if (creatorUser.Result is null || ownerUser.Result is null)
        {
            return Result.Failure<ProjectDetailDto>(Error.User.NotFound());
        }

        if (creatorUser.Result.Role != UserRole.Admin)
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Unauthorized());
        }

        if (ownerUser.Result.IsAdmin() == false || ownerUser.Result.IsAdmin())
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Unauthorized());
        }
        
        var existingProject = await projectRepo.FirstOrDefault(p => p.Code == dto.Code);
        if (existingProject is not null)
        {
            return Result.Failure<ProjectDetailDto>(Error.Project.ProjectCodeNotUnique());
        }

        
        var project = new Project()
        {
            CreatedById = creatorId,
            Name = dto.Name,
            Description = dto.Description,
            Code = dto.Code,
            CurrentPhase = dto.CurrentPhase,
            StartDate = dto.StartDate,
            TargetEndDate = dto.TargetEndDate,
            OwnerName = ownerUser.Result.FullName,
            OwnerId = ownerUser.Result.Id,
            DocumentsCount = 0,
        };
        var trackedProject = await projectRepo.Insert(project);
        
        var projectMemberOwner = new ProjectMember()
        {
            CreatedById = creatorId,
            Email = ownerUser.Result.Email,
            IsOwner = true,
            JoinedAt = DateTime.UtcNow,
            IsActive = true,
            Role = ProjectRole.Manager,
            UserId = dto.OwnerId,
            ProjectId = trackedProject.Id,
        };
        await projectMemberRepo.Insert(projectMemberOwner);
        
        trackedProject.Members.Add(projectMemberOwner);
        
        await unitOfWork.Commit();
        return Result.Success(trackedProject.ToProjectDetailDto());
    }

    public Task<Result<ProjectDetailDto>> UpdateProjectAsync(UpdateProjectDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProjectDetailDto>> AdvanceProjectAsync(AdvancePhaseDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProjectDetailDto>> GetProjectAsync(Guid projectId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedProjectsDto>> GetProjectsAsync(ProjectFilterDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<ProjectSummaryDto>>> GetProjectListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProjectDetailDto>> ArchiveProjectAsync(ArchiveProjectDto dto)
    {
        throw new NotImplementedException();
    }
}