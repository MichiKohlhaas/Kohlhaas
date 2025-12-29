using Kohlhaas.Application.DTO.User;
using Kohlhaas.Application.Interfaces.Token;
using Kohlhaas.Application.Interfaces.User;
using Kohlhaas.Application.Mappings;
using Kohlhaas.Common.Result;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kohlhaas.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    : IUserService
{
    public async Task<Result<UserLoginResponseDto>> LoginUserAsync(UserLoginRequestDto dto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        
        var user = await userRepo.SingleOrDefault(u => u.Email == dto.Email);
        if (user is null)
        {
            return Result.Failure<UserLoginResponseDto>(Error.User.InvalidEmail(dto.Email));
        }
        
        if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) 
            == PasswordVerificationResult.Failed)
        {
            return Result.Failure<UserLoginResponseDto>(Error.User.InvalidPassword(dto.Email));
        }
        
        user.LastLoginAt = DateTime.UtcNow;
        await userRepo.Update(user);
        await unitOfWork.Commit();
        
        var token = tokenService.GenerateToken(user);
        
        return Result.Success(user.ToLoginResponseDto(token));        
    }

    public async Task<Result<UserDetailDto>> RegisterUserAsync(RegisterUserDto dto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        if (await UserEmailExistsAsync(dto.Email))
        {
            return Result.Failure<UserDetailDto>(Error.User.EmailAlreadyExists());
        }
        
        var user = new User()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Role = dto.Role,
            Department = dto.Department ?? string.Empty,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        
        var trackedUser = await userRepo.Insert(user);
        await unitOfWork.Commit();
        return Result.Success(trackedUser.ToDetailDto());
    }

    public async Task<bool> UserEmailExistsAsync(string email)
    {
        return await unitOfWork.GetRepository<User>().Any(u => u.Email == email);
    }

    public Task<Result<UserDetailDto>> GetUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDetailDto>> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<UserSummaryDto>>> GetUserListAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedUsersDto>> GetUsersAsync(UserFilterDto filter)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDetailDto>> UpdateUserProfileAsync(UpdateUserProfileDto profileDto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDetailDto>> UpdateUserRoleAsync(UpdateUserRoleDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ChangeUserPasswordAsync(ChangeUserPasswordDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDetailDto>> ReactivateUserAsync(ReactivateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> PatchUserAsync(Guid userId, PatchUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeactivateUserAsync(DeactivateUserDto dto)
    {
        throw new NotImplementedException();
    }
}