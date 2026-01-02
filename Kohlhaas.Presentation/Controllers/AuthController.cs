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
        return result.IsSuccess? Ok(result.Value) : BadRequest(result.Error.Message);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
                
        if (userId.IsSuccess == false) return NotFound(userId.Error.Message);
        var result = await _userService.GetUserAsync(userId.Value);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error.Message });
    }
    
    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var userId = GetCurrentUserId();
        
        if (userId.IsSuccess == false) return NotFound(userId.Error.Message);
        var result = await _userService.UpdateUserProfileAsync(userId.Value, dto);
        
        if (result.IsSuccess) return Ok(result.Value);
        
        return result.Error.Code switch
        {
            "Error.User.NotFound" => NotFound(new { error = result.Error.Message }),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(
        Guid userId,
        [FromBody] UserRole role)
    {
        // Extract current user ID (should be admin)
        var currentUserResult = GetCurrentUserId();
        if (currentUserResult.IsSuccess == false) return NotFound(currentUserResult.Error.Message);
        
        var result = await _userService.UpdateUserRoleAsync(currentUserResult.Value, userId, role);
        
        if (result.IsSuccess) return Ok(result.Value);
        
        return result.Error.Code switch
        {
            "Error.User.NotFound" => NotFound(new { error = result.Error.Message }),
            "Error.Authorization.Unauthorized" => Forbid(),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> UpdateUserPassword([FromBody] ChangeUserPasswordDto dto)
    {
        var userResult = GetCurrentUserId();
        if (userResult.IsSuccess == false) return NotFound(userResult.Error.Message);

        var passwordResult = await _userService.ChangeUserPasswordAsync(userResult.Value, dto);
        if (passwordResult.IsSuccess) return Ok();
        return passwordResult.Error.Code switch
        {
            "Error.User.NotFound" => NotFound(new { error = passwordResult.Error.Message }),
            "Error.User.InvalidCredentials" => Unauthorized(new { error = passwordResult.Error.Message }),
            _ => BadRequest(new { error = passwordResult.Error.Message })
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid userId, [FromBody] string reason)
    {
        var userResult = GetCurrentUserId();
        if (userResult.IsSuccess == false) return NotFound(userResult.Error.Message);

        var deactivateResult = await _userService.DeactivateUserAsync(userResult.Value, userId, reason);
        if (deactivateResult.IsSuccess) return Ok(new { message = "User was deactivated" });

        return deactivateResult.Error.Code switch
        {
            "Error.User.NotFound" => NotFound(new { error = deactivateResult.Error.Message }),
            "Error.User.DeactivateSelf" => BadRequest(new { error = deactivateResult.Error.Message }),
            "Error.User.Unauthorized" => Forbid(),
            _ => BadRequest(new { error = deactivateResult.Error.Message })
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/reactivate")]
    public async Task<IActionResult> ReactivateUser(Guid userId, [FromBody] string reason)
    {
        var userResult = GetCurrentUserId();
        if (userResult.IsSuccess == false) return NotFound(userResult.Error.Message);
        
        var reactivateResult = await _userService.ReactivateUserAsync(userResult.Value, userId, reason);
        if (reactivateResult.IsSuccess) return Ok(new { message = "User was reactivated" });

        return reactivateResult.Error.Code switch
        {
            "Error.User.NotFound" => NotFound(new { error = reactivateResult.Error.Message }),
            "Error.User.Unauthorized" => Forbid(),
            _ => BadRequest(new { error = reactivateResult.Error.Message })
        };
    }
}