using Readora.Models;
using Readora.Models.Authentification;

namespace Readora.Services.Abstractions;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<(string, User)> LoginAsync(LoginRequest request);
    Task<User?> CheckAsync(Guid id);
}