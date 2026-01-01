using Kohlhaas.Application.DTO.User.Supporting;
using Kohlhaas.Domain.Entities;

namespace Kohlhaas.Application.Mappings;

public static class RefreshTokenMappingExtensions
{
    extension(RefreshToken token)
    {
        public RefreshTokenDto ToRefreshTokenDto()
        {
            return new RefreshTokenDto
            {
                ExpiresAtUtc = token.ExpiresAt,
                Token = token.Token,
                TokenId = token.Id,
                UserId = token.UserId
            };
        }
    }
}