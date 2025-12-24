using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.Interfaces.User;

public interface ICurrentUserService
{
    Guid GetUserId();
    string GetEmail();
    UserRole GetRole();
    bool IsInRole(UserRole minimumRole);
    bool IsAuthenticated();
}