using Kohlhaas.Common.Result;
using Kohlhaas.Application.DTO.User;

namespace Kohlhaas.Application.Interfaces.User;

/// <summary>
/// Service for processing User requests and directing them to the appropriate layer.
/// </summary>
public interface IUserService
{
    /* ========== Login ========== */
    /// <summary>
    /// Might move to a separate Authentication service when needed.
    /// </summary>
    /// <param name="email">The user's email that they registered with</param>
    /// <param name="password">The user's password</param>
    /// <returns>The logged in user detail</returns>
    Task<Result<UserDetailDto>> LoginUserAsync(string email, string password);
    
    /* ========== Create ========== */
    /// <summary>
    /// For registering a new user in the system.
    /// </summary>
    /// <param name="dto">User information</param>
    /// <returns>The successfully created user</returns>
    Task<Result<UserDetailDto>> RegisterUserAsync(RegisterUserDto dto);
    /// <summary>
    /// Related to <see cref="RegisterUserAsync"/>. If user resets password using a link sent to email,
    /// can't have duplicate emails in DB.
    /// UI can use to show if yes/no if email is accepted--maybe.
    /// </summary>
    /// <param name="email"></param>
    /// <returns>True if available</returns>
    Task<bool> UserEmailAvailable(string email);
    
    /* ========== Read ========== */
    /// <summary>
    /// When we (who is we?) need info about a single user.
    /// </summary>
    /// <param name="id">The user's ID</param>
    /// <returns>Details about the user</returns>
    Task<Result<UserDetailDto>> GetUserAsync(Guid id);
    /// <summary>
    /// User lookup by email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<Result<UserDetailDto>> GetUserByEmailAsync(string email);
    /// <summary>
    /// Support for listview operations.
    /// </summary>
    /// <returns>List summary of users</returns>
    Task<Result<IList<UserSummaryDto>>> GetUserListAsync();
    /// <summary>
    /// Query for specific user(s) matching a pattern.
    /// </summary>
    /// <param name="filter">Filter for informing the system which data to sift out</param>
    /// <returns>Paged-data about the user(s)</returns>
    Task<Result<PagedUsersDto>> GetUsersAsync(UserFilterDto filter);
    
    /* ========== Update ========== */
    /// <summary>
    /// To update the user. Takes in a <see cref="UpdateUserProfileDto"/> object.
    /// </summary>
    /// <param name="profileDto">The user data that is to be updated</param>
    /// <returns>The updated user</returns>
    Task<Result<UserDetailDto>> UpdateUserProfileAsync(UpdateUserProfileDto profileDto);
    /// <summary>
    /// In case the user's role is de/promoted. Takes in a <see cref="UpdateUserRoleDto"/>
    /// </summary>
    /// <param name="dto">The user's new role data</param>
    /// <returns>The updated user</returns>
    Task<Result<UserDetailDto>> UpdateUserRoleAsync(UpdateUserRoleDto dto);
    /// <summary>
    /// User's need a mechanism to change their password. Takes in a <see cref="ChangeUserPasswordDto"/>.
    /// Could move to something like an Authentication service
    /// </summary>
    /// <param name="dto">The user's new password</param>
    /// <returns>Success if the password is changed</returns>
    Task<Result> ChangeUserPasswordAsync(ChangeUserPasswordDto dto);
    /// <summary>
    /// If a user's account was deactivated, we need a method to reactivate it.
    /// </summary>
    /// <param name="dto">The data about the user to reactivate</param>
    /// <returns>The reactivated user</returns>
    Task<Result<UserDetailDto>> ReactivateUserAsync(ReactivateUserDto dto);
    
    /* ========== Patch ========== */
    /// <summary>
    /// JSON PATCH operation for partial updating rather than updating the entire User.
    /// NOTE FOR SELF: not a full implementation of JSON PATCH (for now...) 
    /// </summary>
    /// <param name="userId">The user that is being udpated</param>
    /// <param name="dto">Data that are to be updated</param>
    /// <returns>Result with success</returns>
    Task<Result> PatchUserAsync(Guid userId, PatchUserDto dto);
    
    /* ========== Delete ========== */
    /// <summary>
    /// Deactivate a user's account to lock them out of the system.
    /// </summary>
    /// <param name="dto">User deactivation data</param>
    /// <returns>Success if no error</returns>
    Task<Result> DeactivateUserAsync(DeactivateUserDto dto);
}