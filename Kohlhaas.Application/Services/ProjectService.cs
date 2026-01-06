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
    public async Task<Result<ProjectMemberDetailDto>> AssignToProjectAsync(Guid assignerId, CreateProjectMemberDto dto)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var projectTask = projectRepo.GetById(dto.ProjectId);
        var assigneeTask = userRepo.GetById(dto.UserId);
        var assignerTask = userRepo.GetById(assignerId);
        
        await Task.WhenAll(assigneeTask, projectTask, assignerTask);
        if (projectTask.Result is null || assigneeTask.Result is null || assignerTask.Result is null)
        {
            var error = projectTask.Result is null 
                ? Error.Project.ProjectIdNotFound() 
                : Error.User.NotFound();
            return Result.Failure<ProjectMemberDetailDto>(error);
        }
        
        var assigner = assignerTask.Result;
        var assignee = assigneeTask.Result;
        var project = projectTask.Result;
        if (project.IsArchived)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.Project.ProjectArchived());
        }
        var assignerProMem = await projectMemberRepo.FirstOrDefault(pm => pm.UserId == assignerId && pm.ProjectId == dto.ProjectId);
        var canAddMembers = assigner.IsAdmin()
                            || assignerProMem is { IsOwner: true }
                            || assignerProMem is { Role: >= ProjectRole.Manager };
        
        if (canAddMembers == false)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.Authorization.Forbidden());
        }
        
        if (await projectMemberRepo.Any(pm => pm.ProjectId == dto.ProjectId && pm.UserId == dto.UserId))
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.ProjectMember.AlreadyProjectMember());
        }

        var projectMember = new ProjectMember
        {
            ProjectId = dto.ProjectId,
            UserId = dto.UserId,
            Role = dto.Role,
            Email = assignee.Email,
            IsOwner = false,
            JoinedAt = DateTime.UtcNow,
            IsActive = true,
        };
        var trackedProjectMember = await projectMemberRepo.Insert(projectMember);
        await unitOfWork.Commit();
        
        return Result.Success(trackedProjectMember.ToProjectMemberDetailDto(assignee.FullName));
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
        
        if (project.IsArchived)
        {
            return Result.Failure<ProjectDetailDto>(Error.Project.ProjectArchived());
        }

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

    public async Task<Result<ProjectDetailDto>> ArchiveProjectAsync(Guid archiver, ArchiveProjectDto dto)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var projectTask = projectRepo.GetById(dto.ProjectId);
        var userTask = userRepo.GetById(archiver);
        await Task.WhenAll(userTask, projectTask);

        if (projectTask.Result is null || userTask.Result is null)
        {
            var error = projectTask.Result is null ?  Error.Project.ProjectIdNotFound() : Error.User.NotFound();
            return Result.Failure<ProjectDetailDto>(error);
        }
        var user = userTask.Result;
        var project = projectTask.Result;

        if (project.IsArchived)
        {
            /* we could just return the success with the DTO 'cause archiving a project is an idempotent operation
               but an error is more explicit. 
            */
            return Result.Failure<ProjectDetailDto>(Error.Project.ProjectArchived());
        }
        
        var documentRepo = unitOfWork.GetRepository<Document>();
        if (await documentRepo.Any(d =>
                d.Status <= DocumentStatus.InReview
                && d.ProjectId == project.Id))
        {
            return Result.Failure<ProjectDetailDto>(Error.Project.ProjectOutstandingDocuments());
        }
        
        // who can archive? Only project owner
        var projectMember = await projectMemberRepo.FirstOrDefault(pm => 
            pm.UserId == user.Id 
            && pm.ProjectId == project.Id 
            && pm.IsActive == true);
        
        if (projectMember is null || project.CanBeArchivedBy(projectMember) == false)
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Forbidden());
        }
        
        project.ArchiveNotes = dto.Notes;
        project.IsArchived = true;
        project.ArchiveReason = dto.ArchiveReason;

        var members = await projectMemberRepo.Get(pm => pm.ProjectId == project.Id);
        foreach (var pm in members)
        {
            if (pm.IsOwner) continue;
            pm.IsActive = false;
        }
            
        await projectMemberRepo.Update(members);
        await projectRepo.Update(project);
        await unitOfWork.Commit();
        return Result.Success(project.ToProjectDetailDto());
    }
}