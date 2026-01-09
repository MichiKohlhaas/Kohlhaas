using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Application.DTO.ProjectMember;
using Kohlhaas.Common.Result;

namespace Kohlhaas.Application.Interfaces.Project;

public interface IProjectService
{
    /* ========== ProjectMember ========== */
    /// <summary>
    /// Link a user to project by making them a member of that project.
    /// </summary>
    /// <param name="assignerId">The user assigning a new project member</param>
    /// <param name="dto">Data needed for the linkage</param>
    /// <returns></returns>
    Task<Result<ProjectMemberDetailDto>> AssignToProjectAsync(Guid assignerId, CreateProjectMemberDto dto);

    /// <summary>
    /// For when there is a need to update a project member's role in the project.
    /// </summary>
    /// <param name="userId">The user updating the project member's role</param>
    /// <param name="dto">Data about the new role.</param>
    /// <returns>The project member object with the new role</returns>
    Task<Result<ProjectMemberDetailDto>> UpdateProjectRoleAsync(Guid userId, UpdateProjectMemberRoleDto dto);

    //Task<Result<ProjectMemberDetailDto>> UpdateProjectMemberAsync(Guid userId, Up);
    /// <summary>
    /// Removes a user from a project
    /// </summary>
    /// <param name="removerId">The user removing the project member</param>
    /// <param name="projectId">The project ID</param>
    /// <param name="userId">The user to remove</param>
    /// <returns>Success if removed</returns>
    Task<Result> RemoveFromProjectAsync(Guid removerId, Guid projectId, Guid userId);
    /// <summary>
    /// Get a user's membership details from a specific project.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="memberId"></param>
    /// <returns>Details about the project member</returns>
    Task<Result<ProjectMemberDetailDto>> GetProjectMemberAsync(Guid projectId, Guid memberId);
    /// <summary>
    /// Gets all members of a project.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <returns>A list of project members</returns>
    Task<Result<IList<ProjectMemberSummaryDto>>> GetProjectMembersAsync(Guid projectId);
    /// <summary>
    /// Get all projects that a user belongs to
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>A list of projects</returns>
    Task<Result<IList<ProjectSummaryDto>>> GetUsersProjectsAsync(Guid userId);
    /// <summary>
    /// Check if user is a member of project before authorizing their action.
    /// </summary>
    /// <param name="projectId">The project's ID</param>
    /// <param name="userId">The user's ID</param>
    /// <returns>True if the user belongs to the project</returns>
    Task<Result<bool>> IsProjectMemberAsync(Guid projectId, Guid userId);

    /// <summary>
    /// Transfer project ownership to someone else.
    /// </summary>
    /// <param name="userId">The user transferring ownership</param>
    /// <param name="projectId">The project ID</param>
    /// <param name="newOwnerId">The new owner's ID; must be member of project</param>
    /// <returns>The updated project</returns>
    Task<Result<ProjectDetailDto>> TransferOwnershipAsync(Guid userId, Guid projectId, Guid newOwnerId);
    
    /// <summary>
    /// Deactivate project member.
    /// </summary>
    /// <param name="userId">The user deactivating the project member</param>
    /// <param name="projectId">The project the member belongs to</param>
    /// <param name="projectMemberId">The user to deactivate</param>
    /// <returns>The deactivated user</returns>
    Task<Result<ProjectMemberDetailDto>> DeactivateProjectMemberAsync(Guid userId, Guid projectId, Guid projectMemberId);
    
    /// <summary>
    /// Reactivate a project member 
    /// </summary>
    /// <param name="userId">The user reactivating the project member</param>
    /// <param name="projectId">The project the member belongs to</param>
    /// <param name="projectMemberId">The user to reactivate</param>
    /// <returns>The reactivated user</returns>
    Task<Result<ProjectMemberDetailDto>> ReactivateProjectMemberAsync(Guid userId, Guid projectId, Guid projectMemberId);
    
    /* ========== Project ========== */
    /// <summary>
    /// Creates a new project. Must specify the owner.
    /// </summary>
    /// <param name="creatorId">The user who created the project</param>
    /// <param name="dto">Project data object</param>
    /// <returns>The created project</returns>
    Task<Result<ProjectDetailDto>> CreateProjectAsync(Guid creatorId, CreateProjectDto dto);

    /// <summary>
    /// Updates the project's mutable information. 
    /// </summary>
    /// <param name="updaterId">The ID of the user updating the project</param>
    /// <param name="dto">The updated project data</param>
    /// <returns>The updated project</returns>
    Task<Result<ProjectDetailDto>> UpdateProjectAsync(Guid updaterId, UpdateProjectDto dto);
    /// <summary>
    /// Move project to the next phase in the V-Model.
    /// </summary>
    /// <param name="dto">The phase advancement data</param>
    /// <returns>The project advanced to the next phase</returns>
    Task<Result<ProjectDetailDto>> AdvanceProjectAsync(AdvancePhaseDto dto);

    /// <summary>
    /// Get a single project that matches the ID.
    /// </summary>
    /// <param name="userId">The user making the request</param>
    /// <param name="projectId">The project ID</param>
    /// <returns>The project matching the ID if it exists</returns>
    Task<Result<ProjectDetailDto>> GetProjectAsync(Guid userId, Guid projectId);

    /// <summary>
    /// Get a paged list of projects
    /// </summary>
    /// <param name="userId">The user making the request</param>
    /// <param name="dto">Paged project data</param>
    /// <returns>Paged project collection result</returns>
    Task<Result<PagedProjectsDto>> GetProjectsAsync(Guid userId, ProjectFilterDto dto);
    /// <summary>
    /// List view operation for retrieving all projects.
    /// </summary>
    /// <returns>A list of all projects</returns>
    Task<Result<IList<ProjectSummaryDto>>> GetProjectListAsync();

    /// <summary>
    /// For when a project is completed, archived, or cancelled.
    /// </summary>
    /// <param name="archiver">The ID of the user archiving the project</param>
    /// <param name="dto"></param>
    /// <returns>The archived project</returns>
    Task<Result<ProjectDetailDto>> ArchiveProjectAsync(Guid archiver, ArchiveProjectDto dto);
}