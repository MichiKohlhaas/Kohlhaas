namespace Kohlhaas.Application.Interfaces.Security;

public interface IPasswordHasher
{
    string HashPassword(Domain.Entities.User user, string password);
    bool VerifyHashedPassword(Domain.Entities.User user, string hashedPassword, string providedPassword);
}