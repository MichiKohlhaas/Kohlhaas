using Kohlhaas.Application.Interfaces.User;
using Kohlhaas.Domain.Interfaces;

namespace Kohlhaas.Application.Services;

public class UserService
{
    /* ========== Contexts ========== */
    /// <summary>
    /// Context of the user making the request. For authorization.
    /// </summary>
    ICurrentUserService CurrentUser { get; set; }
    
    IUnitOfWork UnitOfWork { get; }
}