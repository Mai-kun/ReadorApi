using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readora.API.DtoModels;
using Readora.DataBase;
using Readora.Models;

namespace Readora.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ReadoraDbContext _context;

    public UsersController(ReadoraDbContext context)
    {
        _context = context;
    }

    // GET: api/Users
    // [HttpGet]
    // public async Task<List<Models.User>> GetUsers()
    // {
    //     return await _context.Users.ToListAsync();
    // }

    // GET: api/Users/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileDto>> GetUser(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Credential)
            .Include(u => u.Books)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();

        return new UserProfileDto
        {
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            PublicKey = user.PublicKey ?? "",
            TotalBooks = user.Books?.Count ?? 0,
            Books = user.Books?.Select(MapToBookDto).ToList() ?? []
        };
    }

    // GET: api/Users/me
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetUser()
    {
        if (!HttpContext.Request.Cookies.TryGetValue(nameof(StringLiterals.UserId), out var userId))
        {
            return BadRequest(); 
        }
        
        var user = await _context.Users
            .Include(u => u.Credential)
            .Include(u => u.Books)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

        if (user == null) return NotFound();

        return new UserProfileDto
        {
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            PublicKey = user.PublicKey ?? "",
            TotalBooks = user.Books?.Count ?? 0,
            Books = user.Books?.Select(MapToBookDto).ToList() ?? []
        };
    }
    
    // PUT: api/Users/5
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutUser(Guid id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest();
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }
    
    // DELETE: api/Users/5
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(Guid id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
    
    private BookDto MapToBookDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            CoverUrl = $"{Request.Scheme}://{Request.Host}/{book.CoverImagePath}",
            UploadDate = book.UploadDate,
            Author = book.Author.Username,
            Genres = book.Genres.Select(g => g.Name).ToList(),
            PublicationYear = 0
        };
    }
}