using Kohlhaas.Application.DTO.User;
using Kohlhaas.Application.Interfaces.User;
using Kohlhaas.Common.Result;
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
        
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequestDto dto)
    {
        var result = await _userService.LoginUserAsync(dto);
        return result.IsSuccess? Ok(result.Value) : HandleError(result);
    }

    [HttpGet]
    public async Task<IActionResult> CheckEmailAvailable([FromBody] string email)
    {
        var isAvailable = await _userService.UserEmailExistsAsync(email);
        return Ok(!isAvailable);
    }

    [Authorize]
    [HttpGet("users/{userId}/profile")]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId.IsSuccess is false) return NotFound(currentUserId.Error.Message);
                
        var result = await _userService.GetUserAsync(userId);
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }
    
    [Authorize]
    [HttpPut("users/{userId}/profile")]
    public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateUserProfileDto dto)
    {
        var currentUserId = ValidateRequest(userId, dto.Id);
        if (currentUserId.IsSuccess is false) return HandleError(currentUserId);
        
        var result = await _userService.UpdateUserProfileAsync(currentUserId.Value, dto);
        
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> UpdateUserRole(
        Guid userId,
        [FromBody] UserRole role)
    {
        var currentUserResult = GetCurrentUserId();
        if (currentUserResult.IsSuccess is false) return NotFound(currentUserResult.Error.Message);
        
        var result = await _userService.UpdateUserRoleAsync(currentUserResult.Value, userId, role);
        
        return result.IsSuccess ? Ok(result.Value) : HandleError(result);
    }

    [Authorize]
    [HttpPost("users/{userId}/change-password")]
    public async Task<IActionResult> UpdateUserPassword(Guid userId, [FromBody] ChangeUserPasswordDto dto)
    {
        var userResult = ValidateRequest(userId, dto.UserId);
        if (userResult.IsSuccess is false) return HandleError(userResult);

        var passwordResult = await _userService.ChangeUserPasswordAsync(userResult.Value, dto);
        return passwordResult.IsSuccess ? Ok() : HandleError(passwordResult);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid userId, [FromBody] string reason)
    {
        var userResult = GetCurrentUserId();
        if (userResult.IsSuccess == false) return NotFound(userResult.Error.Message);

        var deactivateResult = await _userService.DeactivateUserAsync(userResult.Value, userId, reason);
        return deactivateResult.IsSuccess ? Ok(new { message = "User was deactivated" }) : HandleError(deactivateResult);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("users/{userId}/reactivate")]
    public async Task<IActionResult> ReactivateUser(Guid userId, [FromBody] string reason)
    {
        var userResult = GetCurrentUserId();
        if (userResult.IsSuccess == false) return NotFound(userResult.Error.Message);
        
        var reactivateResult = await _userService.ReactivateUserAsync(userResult.Value, userId, reason);
        return reactivateResult.IsSuccess ? Ok(new { message = "User was reactivated" }) : HandleError(reactivateResult);
    }

    protected override IActionResult HandleError(Result result)
    {
        return result.Error.Code switch
        {
            "Error.Authorization.Forbidden" => Forbid(result.Error.Message),
            "Error.Validation.ValidationError" => BadRequest(result.Error.Message),
            "Error.User.NotFound" => NotFound(new { error = result.Error.Message }),
            "Error.User.Unauthorized" => Unauthorized(result.Error.Message),
            "Error.User.DeactivateSelf" => Conflict(new { error = result.Error.Message }),
            "Error.User.EmailAlreadyExists" => Conflict(new { error = result.Error.Message }),
            "Error.User.InvalidCredentials" => Unauthorized(new { error = result.Error.Message }),
            _ => BadRequest(new { error = result.Error.Message })
        };
    }
}