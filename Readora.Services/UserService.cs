using Microsoft.EntityFrameworkCore;
using Readora.DataBase;
using Readora.Models;
using Readora.Services.Abstractions;

namespace Readora.Services;

public class UserService : IUserService
{
    private readonly ReadoraDbContext _readoraDbContext;
    
    public UserService(ReadoraDbContext readoraDbContext)
    {
        _readoraDbContext = readoraDbContext;
    }
    
    Task<User?> IUserService.GetUserByEmailAsync(string email, CancellationToken token)
    {
        return _readoraDbContext.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .Include(user => user.Credential)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken: token);
    }

    public Task<User?> GetUserById(Guid userId, CancellationToken token)
    {
        return _readoraDbContext.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .Include(user => user.Credential)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: token);
    }
}