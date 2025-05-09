using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Readora.API.DtoModels;
using Readora.Models;
using Readora.Models.Authentification;
using Readora.Services.Abstractions;

namespace Readora.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("AllowClient")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken token)
    {
        var jwtToken = await _authService.RegisterAsync(request);
        SetCookie(nameof(StringLiterals.TastyCookies), jwtToken);
        
        var user = await _userService.GetUserByEmailAsync(request.Email, token);
        SetCookie(nameof(StringLiterals.UserId), user!.Id.ToString());
        
        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken token)
    {
        try
        {
            var (jwtToken, user) = await _authService.LoginAsync(request);
            
            SetCookie(nameof(StringLiterals.TastyCookies), jwtToken);
            SetCookie(nameof(StringLiterals.UserId), user.Id.ToString());

            var userDto = new UserDto 
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = new RoleDto 
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                }
            };
        
            return Ok(new { 
                Message = "Login successful", 
                User = userDto 
            });
        }
        catch (UnauthorizedAccessException)
        {
            return new UnauthorizedResult();
        }
    }
    
    [HttpPost("logout")]
    public IActionResult Logout(CancellationToken token)
    {
        HttpContext.Response.Cookies.Delete(nameof(StringLiterals.TastyCookies),  new CookieOptions 
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UnixEpoch
        });
        
        HttpContext.Response.Cookies.Delete(nameof(StringLiterals.UserId),  new CookieOptions 
        {
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UnixEpoch
        });
    
        return Ok(new { message = "Logout successful" });
    }
    
    [HttpGet("check")]
    public async Task<IActionResult> CheckAuth(CancellationToken cancellationToken)
    {
        try 
        {
            if (!HttpContext.Request.Cookies.TryGetValue(nameof(StringLiterals.UserId), out var userId))
            {
                return Unauthorized(); 
            }

            var user = await _userService.GetUserById(Guid.Parse(userId), cancellationToken);

            if (user is null)
            {
                return Unauthorized();
            }
        
            var userDto = new UserDto 
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = new RoleDto 
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                }
            };
        
            return Ok(userDto);
        }
        catch (Exception)
        {
            return Unauthorized();
        }
    }

    private void SetCookie(string name, string value)
    {
        Response.Cookies.Append(name, value, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        });
    }
}