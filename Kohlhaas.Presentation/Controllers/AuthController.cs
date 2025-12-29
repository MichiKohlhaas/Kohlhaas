using Kohlhaas.Application.DTO.User;
using Kohlhaas.Application.Interfaces.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kohlhaas.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    
    [AllowAnonymous]
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

    
}