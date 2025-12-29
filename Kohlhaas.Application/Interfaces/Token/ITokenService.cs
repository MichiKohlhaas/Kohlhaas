namespace Kohlhaas.Application.Interfaces.Token;

public interface ITokenService
{
    string GenerateToken(Domain.Entities.User user);
}