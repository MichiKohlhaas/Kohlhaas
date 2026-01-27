using Kohlhaas.Domain.Entities;
using Kohlhaas.Application.Interfaces.Security;
using Microsoft.AspNetCore.Identity;

namespace Kohlhaas.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private readonly IPasswordHasher<User>  _passwordHasher = new PasswordHasher<User>();

    public string HashPassword(User user, string password) => _passwordHasher.HashPassword(user, password);
    

    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}