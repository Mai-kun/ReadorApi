using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Readora.DataBase;
using Readora.Models;
using Readora.Models.Authentification;
using Readora.Services.Abstractions;

namespace Readora.Services;

public class AuthService : IAuthService
{
    private readonly ReadoraDbContext _context;
    private readonly JwtOptions _jwtOptions;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        ReadoraDbContext context, 
        IOptions<JwtOptions> jwtOptions,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _passwordHasher = passwordHasher;
    }

    async Task<string> IAuthService.RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AsNoTracking().AnyAsync(u => u.Email == request.Email))
            throw new ArgumentException("Email already exists");

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            Role = await _context.Roles.FirstAsync(r => r.Name == "User"),
            Credential = new UserCredential
            {
                PasswordHash = _passwordHasher.HashPassword(request.Password),
            }
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return GenerateJwtToken(user);
    }

    async Task<(string, User)> IAuthService.LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(u => u.Credential)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !_passwordHasher.VerifyPassword(
            request.Password,
            user.Credential.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        return (GenerateJwtToken(user), user);
    }

    async Task<User?> IAuthService.CheckAsync(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Credential)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user ?? null;
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddYears(_jwtOptions.AccessTokenLifetimeYears),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}