using System.Reflection.Metadata.Ecma335;
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

        if (creatorUser.Result.Role != UserRole.Admin || ownerUser.Result.IsAdmin() == false || ownerUser.Result.IsAdmin() == false)
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

    public async Task<Result<ProjectDetailDto>> UpdateProjectAsync(Guid updaterId, UpdateProjectDto dto)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var projectTask = projectRepo.GetById(dto.Id);
        var userTask = userRepo.GetById(updaterId);
        await Task.WhenAll(userTask, projectTask);
        
        if (projectTask.Result is null || userTask.Result is null)
        {
            var error = projectTask.Result is null 
                ? Error.Project.ProjectIdNotFound() 
                : Error.User.NotFound();
            return Result.Failure<ProjectDetailDto>(error);
        }
        
        var user = userTask.Result;
        var project = projectTask.Result;

        // proj mem -> is in proj? -> isOwner or manager? -> isActive? -> ok
        var canManageProject = await projectMemberRepo.Any(pm => pm.UserId == user.Id
                                                                && pm.ProjectId == project.Id
                                                                && (pm.IsOwner == true || pm.Role >= ProjectRole.Manager)
                                                                && pm.IsActive == true);
        
        var mayUpdate = user.IsAdmin() || canManageProject;
        
        if (mayUpdate == false)
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Unauthorized());
        }
        if (project.IsArchived)
        {
            return Result.Failure<ProjectDetailDto>(Error.Project.ProjectArchived());
        }
        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.TargetEndDate = dto.TargetEndDate;

        await projectRepo.Update(project);
        await unitOfWork.Commit();
        return Result.Success(project.ToProjectDetailDto());
    }

    public Task<Result<ProjectDetailDto>> AdvanceProjectAsync(AdvancePhaseDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ProjectDetailDto>> GetProjectAsync(Guid userId, Guid projectId)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var projectTask = projectRepo.GetById(projectId);
        var userTask = userRepo.GetById(userId);
        await Task.WhenAll(userTask, projectTask);

        if (projectTask.Result is null || userTask.Result is null)
        {
            var error = projectTask.Result is null ?  Error.Project.ProjectIdNotFound() : Error.User.NotFound();
            return Result.Failure<ProjectDetailDto>(error);
        }
        
        var project = projectTask.Result;
        var user = userTask.Result;

        if (user.Role == UserRole.Admin)
        {
            return Result.Success(project.ToProjectDetailDto());
        }

        bool isProjectMember = await projectMemberRepo.Any(pm => pm.UserId == user.Id 
                                                             && pm.ProjectId == projectId 
                                                             && pm.IsActive);
        return isProjectMember is not true 
            ? Result.Failure<ProjectDetailDto>(Error.Authorization.Forbidden()) 
            : Result.Success(project.ToProjectDetailDto());
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