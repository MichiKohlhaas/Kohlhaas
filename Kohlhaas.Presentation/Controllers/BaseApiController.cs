using System.Security.Claims;
using Kohlhaas.Common.Result;
using Kohlhaas.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Kohlhaas.Presentation.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected Result<Guid> GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        var isUserId = Guid.TryParse(userIdClaim?.Value, out var userId);
        if (userIdClaim is null || !isUserId)
        {
            Result.Failure<Guid>(Error.Token.TokenUserIdNotFound());
        }

        return Result.Success(userId);
    }

    protected Result<UserRole> GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);

        var isRoleFound = Enum.TryParse<UserRole>(roleClaim?.Value, out var role);
        if (roleClaim is null || !isRoleFound)
        {
            return Result.Failure<UserRole>(Error.Token.TokenUserRoleNotFound());
        }
        return Result.Success(role);
    }

    protected Result<string> GetCurrentUserEmail()
    {
        var emailClaim = User.FindFirst(ClaimTypes.Email);

        return emailClaim is null 
            ? Result.Failure<string>(Error.Token.TokenUserEmailNotFound()) 
            : Result.Success(emailClaim.Value);
    }

    protected bool IsInRole(UserRole role)
    {
        var result = GetCurrentUserRole();
        return result.IsSuccess && result.Value >= role;
    }
}