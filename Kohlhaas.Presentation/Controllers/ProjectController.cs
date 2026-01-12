using Kohlhaas.Application.DTO.Project;
using Kohlhaas.Application.Interfaces.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kohlhaas.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(IProjectService projectService) : BaseApiController
{
    [Authorize]
    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProject(Guid projectId)
    {
        var userId = GetCurrentUserId();
        if (userId.IsSuccess == false) return NotFound(userId.Error.Message);
        
        var result = await projectService.GetProjectAsync(userId.Value, projectId);
        if (result.IsSuccess) return Ok(result.Value);
        return NotFound(result.Error.Message);
    }
    
    [Authorize(Roles = "Admin,ProjectManager")]
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId.IsSuccess == false) return NotFound(userId.Error.Message);

        var result = await projectService.CreateProjectAsync(userId.Value, dto);
        if (result.IsSuccess) return Ok(result.Value);
        
        return result.Error.Code switch
        {
            "Error.Authorization.Forbidden" => Unauthorized(result.Error.Message),
            "Error.Project.ProjectCodeNotUnique" => Conflict(result.Error.Message),
            _ => BadRequest(result.Error.Message)
        };
    }
}