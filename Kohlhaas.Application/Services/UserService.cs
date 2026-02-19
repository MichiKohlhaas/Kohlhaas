using Kohlhaas.Application.DTO.User;
using Kohlhaas.Application.Interfaces.Token;
using Kohlhaas.Application.Interfaces.User;
using Kohlhaas.Application.Mappings;
using Kohlhaas.Common.Result;
using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;
using Kohlhaas.Domain.Interfaces;
using Kohlhaas.Application.Interfaces.Security;
using Microsoft.EntityFrameworkCore;


namespace Kohlhaas.Application.Services;

public class UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService)
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
            == false)
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

    public async Task<Result<UserLoginResponseDto>> RefreshTokenAsync(Guid id, string token)
    {
        var tokenRepo = unitOfWork.GetRepository<RefreshToken>();
        var userRepo = unitOfWork.GetRepository<User>();
        
        var user = await userRepo.SingleOrDefault(u => u.Id == id);
        var refreshToken = await tokenRepo.SingleOrDefault(t => t.Token.Equals(token) && t.UserId == id);

        if (refreshToken is null) return Result.Failure<UserLoginResponseDto>(Error.JwtToken.JwtTokenNotFound());
        if (user is null) return Result.Failure<UserLoginResponseDto>(Error.User.NotFound());
        if (user.IsActive is false) return Result.Failure<UserLoginResponseDto>(Error.User.Deactivated());
        if (refreshToken.ExpiresAt < DateTime.UtcNow) return Result.Failure<UserLoginResponseDto>(Error.JwtToken.JwtTokenExpired());
        
        var jwtToken = tokenService.GenerateToken(user);
        user.LastLoginAt = DateTime.UtcNow;
        await userRepo.Update(user);
        await unitOfWork.Commit();
        
        return Result.Success(user.ToLoginResponseDto(jwtToken, refreshToken));
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
        var userRepo = unitOfWork.GetRepository<User>();
        var query = userRepo.AsQueryable();
        
        var builder = new Utility.DynamicQueryBuilder<User>();

        if (string.IsNullOrWhiteSpace(filter.SearchText) == false)
        {
            builder
                .Where("FirstName", filter.SearchText, "OR")
                .Where("LastName", filter.SearchText, "OR")
                .Where("Email", filter.SearchText, "OR");
        }

        if (filter.Role.HasValue)
        {
            builder.Where("Role", "==", filter.Role.Value);
        }

        if (string.IsNullOrWhiteSpace(filter.Department) == false)
        {
            builder.Where("Department", "==", filter.Department);
        }

        if (filter.IsActive.HasValue)
        {
            builder.Where("IsActive", "==", filter.IsActive.Value);
        }

        if (string.IsNullOrWhiteSpace(filter.SortBy) == false)
        {
            builder.OrderBy(filter.SortBy, filter.SortDescending);
        }
        else
        {
            builder.OrderBy("LastName").OrderBy("FirstName");
        }

        var filteredQuery = builder.ApplyTo(query);
        
        var totalItems = await filteredQuery.CountAsync();
        builder.Paginate(filter.PageNumber, filter.PageSize);
        var pagedQuery = builder.ApplyTo(query);

        var users = await pagedQuery.ToListAsync();
        
        return Result.Success(users.ToPagedDto(totalItems, filter.PageNumber, filter.PageSize));
    }

    public async Task<Result<UserDetailDto>> UpdateUserProfileAsync(Guid currentUserId, UpdateUserProfileDto profileDto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var userTask = userRepo.GetById(currentUserId);
        var updateUserTask = userRepo.GetById(profileDto.Id);
        await Task.WhenAll(userTask, updateUserTask);
        
        if (userTask.Result is null || updateUserTask.Result is null)
        {
            return Result.Failure<UserDetailDto>(Error.User.NotFound());
        }
        
        var user = userTask.Result;
        var updateUser = updateUserTask.Result;

        // If the updater != updatee AND updater != Admin
        if (user.Id != profileDto.Id && user.Role < UserRole.Admin)
        {
            return Result.Failure<UserDetailDto>(Error.Authorization.Forbidden());
        }
        
        updateUser.FirstName = profileDto.FirstName;
        updateUser.LastName = profileDto.LastName;
        bool emailUpdated = !updateUser.Email.Equals(profileDto.Email);
        updateUser.Email = profileDto.Email;
        updateUser.Department = profileDto.Department;
        
        if (emailUpdated)
        {
            var pmRepo = unitOfWork.GetRepository<ProjectMember>();
            var projectMembers = await pmRepo.Get(pm => pm.UserId == profileDto.Id);

            if (projectMembers.Count > 0)
            {
                foreach (var pm in projectMembers)
                {
                    pm.Email = updateUser.Email;
                }
            }
            await pmRepo.Update(projectMembers);
        }
        
        await userRepo.Update(updateUser);
        await unitOfWork.Commit();
        return Result.Success(updateUser.ToDetailDto());
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
        if (verifyPwResult == false)
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

    public async Task<Result<UserDetailDto>> PatchUserAsync(Guid currentUserId, Guid targetUserId, PatchUserDto dto)
    {
        var userRepo = unitOfWork.GetRepository<User>();
        var currentUser = await userRepo.GetById(currentUserId);
        var targetUser = await userRepo.GetById(targetUserId);
        
        if (currentUser == null || targetUser == null)
        {
            return Result.Failure<UserDetailDto>(Error.User.NotFound());    
        }
        
        if (currentUserId != targetUserId && currentUser.Role != UserRole.Admin)
        {
            return Result.Failure<UserDetailDto>(Error.Authorization.Unauthorized());
        }
        
        if (dto.FirstName is not null) targetUser.FirstName = dto.FirstName;
        if (dto.LastName is not null) targetUser.LastName = dto.LastName;
        if (dto.Department is not null) targetUser.Department = dto.Department;
    
        await userRepo.Update(targetUser);
        await unitOfWork.Commit();
    
        return Result.Success(targetUser.ToDetailDto());
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

        if (currentUserId == targetUserId)
        {
            return Result.Failure<UserDetailDto>(Error.User.DeactivateSelf());
        }

        if (adminUser.Role != UserRole.Admin)
        {
            return Result.Failure<UserDetailDto>(Error.Authorization.Unauthorized());
        }
        // Do something with reason, like store in a log
        targetUser.IsActive = false;
        await userRepo.Update(targetUser);
        await unitOfWork.Commit();
        return Result.Success(adminUser.ToDetailDto());
    }
}