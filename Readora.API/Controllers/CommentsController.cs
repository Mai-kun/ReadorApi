using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readora.API.DtoModels;
using Readora.DataBase;
using Readora.Models;

namespace Readora.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ReadoraDbContext _context;

    public CommentsController(ReadoraDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int bookId)
    {
        var comments = await _context.Comments
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => c.BookId == bookId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                User = new
                {
                    Name = c.User!.Username,
                    Id = c.User!.Id,
                },
            })
            .ToListAsync();

        return Ok(comments);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CommentCreateDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return Unauthorized();
        }

        var comment = new Comment
        {
            Text = dto.Text,
            BookId = dto.BookId,
            UserId = Guid.Parse(userId),
        };

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        await _context.Comments
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

        return Ok(new { Message = "Comment was removed"});
    }
}