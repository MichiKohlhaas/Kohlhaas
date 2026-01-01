using Kohlhaas.Application.DTO.User;
using Kohlhaas.Application.Interfaces.User;
using Kohlhaas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kohlhaas.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : BaseApiController
{
    private readonly IUserService _userService = userService;
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _userService.RegisterUserAsync(dto);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error.Code switch
        {
            "Error.User.EmailAlreadyExists" => Conflict(new { error = result.Error.Message }),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequestDto dto)
    {
        var result = await _userService.LoginUserAsync(dto);
        if (result.IsSuccess) return Ok(result.Value);
        return BadRequest(result.Error.Message);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
                
        if (userId.IsSuccess == false) return NotFound(userId.Error.Message);
        var result = await _userService.GetUserAsync(userId.Value);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return NotFound(new { error = result.Error.Message });
    }
    
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userId = GetCurrentUserId();
        
        if (userId.IsSuccess == false) return NotFound(userId.Error.Message);
        var result = await _userService.UpdateUserProfileAsync(userId.Value, dto);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error.Code switch
        {
            "User.NotFound" => NotFound(new { error = result.Error.Message }),
            "Unauthorized" => Forbid(),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{targetUserId}/role")]
    public async Task<IActionResult> UpdateUserRole(
        Guid userId,
        [FromBody] UserRole role)
    {
        // Extract current user ID (should be admin)
        var currentUserResult = GetCurrentUserId();
        if (currentUserResult.IsSuccess == false) return NotFound(currentUserResult.Error.Message);
        
        var result = await _userService.UpdateUserRoleAsync(currentUserResult.Value, userId, role);
        
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        
        return result.Error.Code switch
        {
            "User.NotFound" => NotFound(new { error = result.Error.Message }),
            "Unauthorized" => Forbid(),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }
    
}