using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Application.Interfaces.Project;
using Kohlhaas.Common.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kohlhaas.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService) : BaseApiController
{
    const string RouteIdMismatch = "Route ID does not match body ID";
    
    [Authorize]
    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        var userId = GetCurrentUserId();
        if (userId.IsSuccess is false) return NotFound(userId.Error.Message);
        
        var result = await projectService.GetProjectAsync(userId.Value, projectId);
        if (result.IsSuccess) return Ok(result.Value);
        return NotFound(result.Error.Message);
    }
    
    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId.IsSuccess is false) return NotFound(userId.Error.Message);

        var result = await projectService.CreateProjectAsync(userId.Value, dto);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] UpdateProjectDto dto)
    {
        if (projectId != dto.Id) return BadRequest(RouteIdMismatch);
        var userId = GetCurrentUserId();
        if (userId.IsSuccess is false) return NotFound(userId.Error.Message);

        var result = await projectService.UpdateProjectAsync(userId.Value, dto);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpDelete("{projectId}")]
    public async Task<IActionResult> ArchiveProject(Guid projectId, ArchiveProjectDto dto)
    {
        var userIdResult = ValidateRequest(projectId, dto.ProjectId);
        if (userIdResult.IsSuccess is false) return HandleError(userIdResult);
        
        var result = await projectService.ArchiveProjectAsync(userIdResult.Value, dto);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    protected override IActionResult HandleError(Result result)
    {
        return result.Error.Code switch
        {
            "Error.Authorization.Forbidden" => Forbid(result.Error.Message),
            "Error.Validation.ValidationError" => BadRequest(result.Error.Message),
            "Error.User.NotFound" => NotFound(result.Error.Message),
            "Error.Project.ProjectCodeNotUnique" => Conflict(result.Error.Message),
            "Error.Project.ProjectIdNotFound" => NotFound(result.Error.Message),
            "Error.Project.ProjectArchived" => Conflict(result.Error.Message),
            _ => BadRequest(result.Error.Message)
        };
    }
}