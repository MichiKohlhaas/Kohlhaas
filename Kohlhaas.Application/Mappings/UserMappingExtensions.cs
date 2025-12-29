using Kohlhaas.Application.DTO.User;
using Kohlhaas.Domain.Entities;

namespace Kohlhaas.Application.Mappings;

public static class UserMappingExtensions
{
    extension(User user)
    {
        public UserDetailDto ToDetailDto()
        {
            return new UserDetailDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                LastModifiedAt = user.ModifiedAt,
                IsActive = user.IsActive,
                Role = user.Role,
                Department = user.Department,
            };
        }

        public UserSummaryDto ToSummaryDto()
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role,
                IsActive = user.IsActive,
                Department = user.Department,
                LastLoginAt = user.LastLoginAt,
            };
        }

        public UserLoginResponseDto ToLoginResponseDto(string token)
        {
            return new UserLoginResponseDto
            {
                Token = token,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                LastModifiedAt = user.ModifiedAt,
                IsActive = user.IsActive,
                Role = user.Role,
                Department = user.Department,
            };
        }

        public UserActivityReportDto ToActivityReportDto()
        {
            return new UserActivityReportDto
            {
                LastLoginAt = user.LastLoginAt,
                PasswordChangeAt = user.PasswordChangeAt,
                LastModifiedAt = user.ModifiedAt,
            };
        }
        
    }

    extension(IEnumerable<User> source)
    {
        public IEnumerable<UserDetailDto> ToDetailDtos()
        {
            return source.Select(ToDetailDto);
        }
        
        public IEnumerable<UserSummaryDto> ToSummaryDtos()
        {
            return source.Select(ToSummaryDto);
        }
    }

}