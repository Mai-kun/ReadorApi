using Readora.Models;

namespace Readora.Services.Abstractions;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken token);
    Task<User?> GetUserById(Guid userId, CancellationToken token);
}