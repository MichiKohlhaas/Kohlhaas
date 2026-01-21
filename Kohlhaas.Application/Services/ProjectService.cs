using System.Data.Entity;
using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Application.DTO.ProjectMember;
using Kohlhaas.Application.Interfaces.Project;
using Kohlhaas.Application.Mappings;
using Kohlhaas.Common.Result;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;
using Kohlhaas.Domain.Interfaces;

namespace Kohlhaas.Application.Services;

public sealed class ProjectService(IUnitOfWork unitOfWork) : IProjectService
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
        var assignerProMem = await projectMemberRepo.FirstOrDefault(pm => 
            pm.UserId == assignerId 
            && pm.ProjectId == dto.ProjectId);
        
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
        
        return Result.Success(trackedProjectMember.ToProjectMemberDetailDto());
    }

    public async Task<Result<ProjectMemberDetailDto>> UpdateProjectRoleAsync(Guid userId, UpdateProjectMemberRoleDto dto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var userTask = userRepo.GetById(userId);
        var projMemTask = pmRepo.GetById(dto.MemberId);
        
        await Task.WhenAll(userTask, projMemTask);

        if (userTask.Result is null || projMemTask.Result is null)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.User.NotFound());
        }
        
        var projMem = projMemTask.Result;
        // auth check:
        if (await AuthCheck(pmRepo, userTask.Result, dto.ProjectId) == false)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.Authorization.Forbidden());
        }
        projMem.Role = dto.Role;
        await pmRepo.Update(projMem);
        await unitOfWork.Commit();
        return Result.Success(projMem.ToProjectMemberDetailDto());
    }

    public async Task<Result> RemoveFromProjectAsync(Guid removerId, Guid projectId, Guid userId)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var projectRepo = unitOfWork.GetRepository<Project>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var userTask = userRepo.GetById(userId);
        var projectTask = projectRepo.GetById(projectId);
        var projMemTask = pmRepo.GetById(removerId);
        
        await Task.WhenAll(userTask, projMemTask, projectTask);
        if (userTask.Result is null || projMemTask.Result is null || projectTask.Result is null)
        {
            var error = projectTask.Result is null 
                ? Error.Project.ProjectIdNotFound()
                :  Error.User.NotFound();
            return Result.Failure(error);
        }

        if (await pmRepo.Any(pm => pm.ProjectId == projectId && pm.Id == userId) == false)
        {
            return Result.Failure(Error.Project.NotProjectMember());
        }
        if (await AuthCheck(pmRepo, userTask.Result, projectId) == false)
        {
            return Result.Failure(Error.Authorization.Forbidden());
        }
        
        await pmRepo.HardDelete(userId);
        await unitOfWork.Commit();
        return Result.Success();
    }

    public async Task<Result<ProjectMemberDetailDto>> GetProjectMemberAsync(Guid projectId, Guid memberId)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var pmTask = pmRepo.GetById(memberId);
        var projectTask = projectRepo.GetById(projectId);
        
        await Task.WhenAll(pmTask, pmTask);

        if (pmTask.Result is not null && projectTask.Result is not null)
            return Result.Success(pmTask.Result.ToProjectMemberDetailDto());
        var error = projectTask.Result is null
            ?  Error.Project.ProjectIdNotFound()
            :  Error.User.NotFound();
        return Result.Failure<ProjectMemberDetailDto>(error);

    }

    public async Task<Result<IList<ProjectMemberSummaryDto>>> GetProjectMembersAsync(Guid projectId)
    {
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        var projectRepo = unitOfWork.GetRepository<Project>();
        
        var project = await projectRepo.GetById(projectId);
        if (project is null)
        {
            return Result.Failure<IList<ProjectMemberSummaryDto>>(Error.Project.ProjectIdNotFound());
        }
        var projectMembers = await pmRepo.Get(pm => pm.ProjectId ==  projectId);

        return Result.Success<IList<ProjectMemberSummaryDto>>(projectMembers.ToProjectMemberSummaryDtos().ToList());
    }

    public async Task<Result<IList<ProjectSummaryDto>>> GetUsersProjectsAsync(Guid userId)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var user = await userRepo.GetById(userId);
        if (user is null)
        {
            return Result.Failure<IList<ProjectSummaryDto>>(Error.User.NotFound());
        }
        
        var projectMembers = await pmRepo.Get(pm => pm.UserId == user.Id);
        var projectRepo = unitOfWork.GetRepository<Project>().AsQueryable();
        var builder = new Utility.DynamicQueryBuilder<Project>();
        builder.Where("ProjectId", "Contains", projectMembers.Select(x => x.ProjectId).ToArray());
        var query = builder.ApplyTo(projectRepo);
        var totalItems = await query.ToListAsync();
        return Result.Success<IList<ProjectSummaryDto>>(totalItems.ToProjectSummaryDtos().ToList());
    }

    public async Task<Result<bool>> IsProjectMemberAsync(Guid projectId, Guid userId)
    {
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        return await projectMemberRepo.Any(pm => pm.ProjectId == projectId && pm.UserId == userId);
    }

    public async Task<Result<ProjectDetailDto>> TransferOwnershipAsync(Guid userId, Guid projectId, Guid newOwnerId)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        var projectRepo = unitOfWork.GetRepository<Project>();
        
        var userTask = userRepo.GetById(userId);
        var projectTask = projectRepo.GetById(projectId);
        var pmTask = pmRepo.GetById(newOwnerId);
        
        await Task.WhenAll(pmTask, pmTask, userTask);

        if (pmTask.Result is null || projectTask.Result is null || userTask.Result is null)
        {
            var error = projectTask.Result is null
                ? Error.Project.ProjectIdNotFound()
                :  Error.User.NotFound();
            return Result.Failure<ProjectDetailDto>(error);
        }

        if (await AuthCheck(pmRepo, userTask.Result, projectId) == false)
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Forbidden());
        }


        var oldOwnerId = projectTask.Result.OwnerId;
        var oldOwner = await pmRepo.GetById(oldOwnerId);
        if (oldOwner is not null)
        {
            oldOwner.IsOwner = false;
            await pmRepo.Update(oldOwner);
        }
        projectTask.Result.OwnerId = newOwnerId;
        pmTask.Result.IsOwner = true;
        await pmRepo.Update(pmTask.Result);
        await projectRepo.Update(projectTask.Result);
        await unitOfWork.Commit();
        return Result.Success(projectTask.Result.ToProjectDetailDto());
    }

    public async Task<Result<ProjectMemberDetailDto>> DeactivateProjectMemberAsync(Guid userId, Guid projectId,
        Guid projectMemberId)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var pmTask = pmRepo.GetById(projectMemberId);
        var projectTask = projectRepo.GetById(projectId);
        var userTask = userRepo.GetById(userId);
        await Task.WhenAll(pmTask, pmTask, projectTask, userTask);

        if (pmTask.Result is null || projectTask.Result is null || userTask.Result is null)
        {
            var error = projectTask.Result is null
                ? Error.Project.ProjectIdNotFound()
                : Error.User.NotFound();  
            return Result.Failure<ProjectMemberDetailDto>(error);
        }
        
        // who can deactivate a user? ProjMan and admin
        var user = userTask.Result;
        var projMem = pmTask.Result;
        //var project = projectTask.Result;

        if (projMem.IsActive == false)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.ProjectMember.AlreadyDeactivated());
        }

        var isProjectManager = await pmRepo.FirstOrDefault(pm =>
            pm.UserId == user.Id
            && pm.ProjectId == projectId);

        if (user.IsAdmin() == false
            && isProjectManager is { Role: ProjectRole.Manager })
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.Authorization.Forbidden());
        }
        
        projMem.IsActive = false;
        //projMem.LeftAt = DateTime.UtcNow;
        await pmRepo.Update(projMem);
        await unitOfWork.Commit();
        return Result.Success(projMem.ToProjectMemberDetailDto());
    }

    public async Task<Result<ProjectMemberDetailDto>> ReactivateProjectMemberAsync(Guid userId, Guid projectId, Guid projectMemberId)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var pmTask = pmRepo.GetById(projectMemberId);
        var projectTask = projectRepo.GetById(projectId);
        var userTask = userRepo.GetById(userId);
        await Task.WhenAll(pmTask, pmTask, projectTask, userTask);

        if (pmTask.Result is null || projectTask.Result is null || userTask.Result is null)
        {
            var error = projectTask.Result is null
                ? Error.Project.ProjectIdNotFound()
                : Error.User.NotFound();  
            return Result.Failure<ProjectMemberDetailDto>(error);
        }
        
        // who can reactivate a user? ProjMan and admin
        var user = userTask.Result;
        var projMem = pmTask.Result;
        //var project = projectTask.Result;

        if (await pmRepo.Any(pm => 
                pm.Id == projectMemberId 
                && pm.ProjectId == projectId 
                && pm.IsActive == false) == false)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.Project.NotProjectMember());
        }
        
        if (projMem.IsActive == false)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.ProjectMember.AlreadyActive());
        }

        if (await AuthCheck(pmRepo, user, userId) == false)
        {
            return Result.Failure<ProjectMemberDetailDto>(Error.Authorization.Forbidden());
        }
        
        projMem.IsActive = true;
        //projMem.LeftAt = null;
        await pmRepo.Update(projMem);
        await unitOfWork.Commit();
        return Result.Success(projMem.ToProjectMemberDetailDto());
    }

    private async Task<bool> AuthCheck(IRepository<ProjectMember> pmRepo, User user, Guid projectId)
    {
        var isProjectManager = await pmRepo.FirstOrDefault(pm =>
            pm.UserId == user.Id
            && pm.ProjectId == projectId);

        return (user.IsAdmin() == false && isProjectManager is { Role: ProjectRole.Manager });
    }

    public async Task<Result<ProjectDetailDto>> CreateProjectAsync(Guid creatorId, CreateProjectDto dto)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var projectMemberRepo = unitOfWork.GetRepository<ProjectMember>();
        
        var creatorUser = await userRepo.GetById(creatorId);
        var ownerUser = await userRepo.GetById(dto.OwnerId);
        
        if (creatorUser is null || ownerUser is null)
        {
            return Result.Failure<ProjectDetailDto>(Error.User.NotFound());
        }

        var canCreateProject = ownerUser.IsAdmin() || creatorUser.IsAdmin();
        
        if (canCreateProject == false)
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Forbidden());
        }
        
        var isDuplicateCode = projectRepo.AsQueryable().FirstOrDefault(p => p.Code == dto.Code);
        if (isDuplicateCode is not null)
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
            OwnerName = ownerUser.FullName,
            OwnerId = ownerUser.Id,
            DocumentsCount = 0,
        };
        var trackedProject = await projectRepo.Insert(project);
        
        var projectMemberOwner = new ProjectMember()
        {
            CreatedById = creatorId,
            Email = ownerUser.Email,
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
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Forbidden());
        }
        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.TargetEndDate = dto.TargetEndDate;

        await projectRepo.Update(project);
        await unitOfWork.Commit();
        return Result.Success(project.ToProjectDetailDto());
    }

    public async Task<Result<ProjectDetailDto>> AdvanceProjectAsync(Guid userId, AdvancePhaseDto dto)
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var userRepo = unitOfWork.GetRepository<User>();
        var project = await projectRepo.GetById(dto.ProjectId);
        var user = await userRepo.GetById(userId);
        
        if (project is null || user is null)
        {
            var error = project is null
                ? Error.Project.ProjectIdNotFound()
                : Error.User.NotFound();
            return Result.Failure<ProjectDetailDto>(error);
        }
        
        var pmRepo = unitOfWork.GetRepository<ProjectMember>();
        var canAdvanceProject = await pmRepo.Any(pm => 
            pm.UserId == user.Id
            && pm.ProjectId == project.Id
            && pm.IsActive == true
            && (pm.IsOwner == true || pm.Role >= ProjectRole.Manager));

        if ((canAdvanceProject || user.IsAdmin()) == false)
        {
            return Result.Failure<ProjectDetailDto>(Error.Authorization.Forbidden());
        }
        
        // logic if the phase moves "backwards"?
        project.CurrentPhase = dto.TargetPhase;
        // log the reason dto.Reason
        await projectRepo.Update(project);
        await unitOfWork.Commit();
        return Result.Success(project.ToProjectDetailDto());
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

    public async Task<Result<PagedProjectsDto>> GetProjectsAsync(Guid userId, ProjectFilterDto dto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var projectRepo = unitOfWork.GetRepository<Project>();
        
        var user = await userRepo.GetById(userId);
        if (user is null) return  Result.Failure<PagedProjectsDto>(Error.User.NotFound());
        
        var query = projectRepo.AsQueryable();
        var builder = new Utility.DynamicQueryBuilder<Project>();
        
        if (dto.CurrentPhase is not null)
        {
            builder.Where("CurrentPhase", "==", dto.CurrentPhase);
        }

        if (dto.StartDateAfter is not null)
        {
            builder.Where("StartDateAfter", ">=", dto.StartDateAfter);
        }

        if (dto.StartDateBefore is not null)
        {
            builder.Where("StartDateBefore", "<=", dto.StartDateBefore);
        }

        if (string.IsNullOrWhiteSpace(dto.SearchText) == false)
        {
            builder
                .Where("Name", dto.SearchText, "OR")
                .Where("Description", dto.SearchText, "OR")
                .Where("Code", dto.SearchText, "OR")
                .Where("OwnerName", dto.SearchText, "OR");
        }

        if (string.IsNullOrWhiteSpace(dto.SortBy) == false)
        {
            builder.OrderBy(dto.SortBy, dto.SortDescending);
        }
        else
        {
            builder.OrderBy("Name");
        }
        
        var filteredQuery = builder.ApplyTo(query);
        var totalItems = await filteredQuery.CountAsync();
        builder.Paginate(dto.PageNumber, dto.PageSize);
        var pagedQuery = builder.ApplyTo(filteredQuery);
        
        var projects = await pagedQuery.ToListAsync();

        return Result.Success(projects.ToPagedProjectsDto(totalItems, dto.PageNumber, dto.PageSize));
    }

    public async Task<Result<IList<ProjectSummaryDto>>> GetProjectListAsync()
    {
        var projectRepo = unitOfWork.GetRepository<Project>();
        var projects = await projectRepo.GetAll();
        return Result.Success<IList<ProjectSummaryDto>>(projects.ToProjectSummaryDtos().ToList());
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