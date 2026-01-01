using Kohlhaas.Application.DTO.User;
using Kohlhaas.Application.Interfaces.Token;
using Kohlhaas.Application.Interfaces.User;
using Kohlhaas.Application.Mappings;
using Kohlhaas.Common.Result;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;
using Kohlhaas.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Kohlhaas.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher, ITokenService tokenService)
    : IUserService
{
    /// <summary>
    /// <inheritdoc cref="IUserService.LoginUserAsync" path=""/>
    /// </summary>
    /// <param name="dto"></param>
    /// <inheritdoc/>
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

        if (user.IsActive == false)
        {
            return Result.Failure<UserLoginResponseDto>(Error.User.Deactivated());
        }
        
        user.LastLoginAt = DateTime.UtcNow;
        await userRepo.Update(user);
        
        var token = tokenService.GenerateToken(user);
        var refreshToken = new RefreshToken()
        {
            Token = tokenService.GenerateRefreshToken(),
            UserId = user.Id,
            ExpiresAt =  DateTime.UtcNow.AddDays(7),
        };
        await unitOfWork.GetRepository<RefreshToken>().Insert(refreshToken);
        await unitOfWork.Commit();
        
        return Result.Success(user.ToLoginResponseDto(token, refreshToken));
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

    public async Task<Result<UserDetailDto>> GetUserAsync(Guid id)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var user = await userRepo.GetById(id);

        return user is null 
            ? Result.Failure<UserDetailDto>(Error.User.NotFound()) 
            : Result.Success(user.ToDetailDto());
    }

    public async Task<Result<UserDetailDto>> GetUserByEmailAsync(string email)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var user = await userRepo.FirstOrDefault(u => u.Email == email);
        
        return user is null
            ? Result.Failure<UserDetailDto>(Error.User.NotFound())
            : Result.Success(user.ToDetailDto());
    }

    public async Task<Result<IList<UserSummaryDto>>> GetUserListAsync()
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var users = await userRepo.GetAll();
        // var users = await userRepo.Get(predicate: u => u.IsActive, orderBy: q => q.OrderBy(u => u.LastName));
        return Result.Success<IList<UserSummaryDto>>(users.ToSummaryDtos().ToList());
    }

    public async Task<Result<PagedUsersDto>> GetUsersAsync(Guid currentUserId, UserFilterDto filter)
    {
        throw new NotImplementedException();
        /*var userRepo = unitOfWork.GetRepository<User>();
        var users = await userRepo.GetPagedData(
            pageNumber: filter.PageNumber,
            pageSize: filter.PageSize,
            predicate: p => p.IsActive == filter.IsActive,
            orderBy: q => q.OrderByDescending(u => u.LastLoginAt));
        return Result.Success(users.Items.ToPagedDto(users.TotalCount, filter.PageNumber,users.TotalPages));*/
    }

    public async Task<Result<UserDetailDto>> UpdateUserProfileAsync(Guid currentUserId, UpdateUserProfileDto profileDto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var user = await userRepo.GetById(currentUserId);
        if (user == null)
        {
            return Result.Failure<UserDetailDto>(Error.User.NotFound());
        }
        
        user.FirstName = profileDto.FirstName;
        user.LastName = profileDto.LastName;
        user.Email = profileDto.Email;
        user.Department = profileDto.Department;
        
        await userRepo.Update(user);
        await unitOfWork.Commit();
        return Result.Success(user.ToDetailDto());
    }

    public async Task<Result<UserDetailDto>> UpdateUserRoleAsync(Guid currentUserId, Guid targetId, UserRole newRole)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var adminUser = await userRepo.GetById(currentUserId);
        var targetUser = await userRepo.GetById(targetId);

        if (adminUser == null || targetUser == null)
        {
            return Result.Failure<UserDetailDto>(Error.User.NotFound());
        }

        if (adminUser.Role != UserRole.Admin)
        {
            return Result.Failure<UserDetailDto>(Error.Authorization.Unauthorized());
        }
        
        adminUser.Role = newRole;
        await userRepo.Update(adminUser);
        await unitOfWork.Commit();
        return Result.Success(adminUser.ToDetailDto());
    }

    public async Task<Result> ChangeUserPasswordAsync(Guid currentUserId, ChangeUserPasswordDto dto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var user = await userRepo.GetById(currentUserId);

        if (user == null)
        {
            return Result.Failure(Error.User.NotFound());
        }

        var verifyPwResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
        if (verifyPwResult != PasswordVerificationResult.Failed)
        {
            return Result.Failure(Error.User.InvalidCredentials());
        }
        
        user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);
        user.PasswordChangeAt = DateTime.UtcNow;
        
        await userRepo.Update(user);
        await unitOfWork.Commit();
        return Result.Success();
    }

    public async Task<Result<UserDetailDto>> ReactivateUserAsync(Guid currentUserId,  Guid targetUserId, string reason)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var adminUser = await userRepo.GetById(currentUserId);
        var targetUser = await userRepo.GetById(targetUserId);
        if (adminUser == null || targetUser == null)
        {
            return Result.Failure<UserDetailDto>(Error.User.NotFound());
        }

        if (adminUser.Role != UserRole.Admin)
        {
            return Result.Failure<UserDetailDto>(Error.Authorization.Unauthorized());
        }
        targetUser.IsActive = true;
        await userRepo.Update(targetUser);
        await unitOfWork.Commit();
        return Result.Success(adminUser.ToDetailDto());
    }

    public Task<Result> PatchUserAsync(Guid userId, PatchUserDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeactivateUserAsync(Guid currentUserId,  Guid targetUserId, string reason)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var adminUser = await userRepo.GetById(currentUserId);
        var targetUser = await userRepo.GetById(targetUserId);
        if (adminUser == null || targetUser == null)
        {
            return Result.Failure<UserDetailDto>(Error.User.NotFound());
        }

        if (adminUser.Role != UserRole.Admin)
        {
            return Result.Failure<UserDetailDto>(Error.Authorization.Unauthorized());
        }
        // Do something with reason
        targetUser.IsActive = false;
        await userRepo.Update(targetUser);
        await unitOfWork.Commit();
        return Result.Success(adminUser.ToDetailDto());
    }
}