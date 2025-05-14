using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readora.API.DtoModels;
using Readora.DataBase;
using Readora.Models;
using Readora.Models.Enums;
using Readora.Services.Abstractions;

namespace Readora.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController(ReadoraDbContext context, IWebHostEnvironment env, IFileSaver fileSaver) : ControllerBase
{
    // GET: api/Books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromQuery] string? genre)
    {
        var query = context.Books
            .Include(b => b.Genres)
            .Include(b => b.Author)
            .AsQueryable();

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(b => b.Genres.Any(g => g.Name == genre));
        }

        var books = await query
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author!.Username,
                Description = b.Description ?? "",
                CoverUrl = $"{Request.Scheme}://{Request.Host}/{b.CoverImagePath.TrimStart('/')}",
                Genres = b.Genres.Select(g => g.Name)
                    .ToList(),
                UploadDate = b.UploadDate,
                PublicationYear = b.PublicationYear,
                Isbn = b.Isbn,
            })
            .ToListAsync();

        return Ok(books);
    }

    // GET: api/Books/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await context.Books
            .AsNoTracking()
            .Include(b => b.Genres)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(book => book.Id == id);

        if (book is null)
        {
            return NotFound();
        }

        var response = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author!.Username,
            Description = book.Description ?? "",
            CoverUrl = $"{Request.Scheme}://{Request.Host}/{book.CoverImagePath.TrimStart('/')}",
            Genres = book.Genres.Select(g => g.Name)
                .ToList(),
            UploadDate = book.UploadDate,
            PublicationYear = book.PublicationYear,
            Isbn = book.Isbn,
        };

        return Ok(response);
    }

    // PUT: api/Books/5
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutBook(int id, Book book)
    {
        if (id != book.Id)
        {
            return BadRequest();
        }

        context.Entry(book).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // POST: api/Books
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Book>> PostBook([FromForm] BookUploadDto dto, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
        {
            return Unauthorized();
        }

        var bookFile = await fileSaver.SaveFile(dto.BookFile, env.ContentRootPath, "books");
        var coverImage = await fileSaver.SaveFile(dto.CoverImage, env.ContentRootPath,"covers");
        
        var book = new Book
        {
            Title = dto.Title,
            Description = dto.Description,
            PublicationYear = dto.PublicationYear,
            Isbn = dto.Isbn,
            FilePath = @"files\" + bookFile.filePath,
            FileHash = bookFile.hash,
            CoverImagePath = @"files\" + coverImage.filePath,
            AuthorId = Guid.Parse(userId),
            Genres = await context.Genres.Where(g => dto.Genres.Contains(g.Id))
                .ToListAsync(cancellationToken: cancellationToken),
        };

        
        var bookEntity = await context.Books.AddAsync(book, cancellationToken);
        var request = new ModerationRequest
        {
            BookId = bookEntity.Entity.Id,
            Book = bookEntity.Entity,
            Status = ModerationStatus.Pending,
        };
        await context.ModerationRequests.AddAsync(request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return CreatedAtAction("GetBook", new { id = book.Id }, book);
    }

    // DELETE: api/Books/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        context.Books.Remove(book);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id:int}/text")]
    public async Task<IActionResult> GetBookText(int id)
    {
        var book = await context.Books.FindAsync(id);
        if (book == null || string.IsNullOrEmpty(book.FilePath))
            return NotFound();

        var bookPath = Path.Combine(env.ContentRootPath, book.FilePath);
        if (!System.IO.File.Exists(bookPath))
            return NotFound();

        var encoding = Encoding.GetEncoding("windows-1251");
        var text = await System.IO.File.ReadAllTextAsync(bookPath, encoding);

        return Ok(new { content = text });
    }

    
    private bool BookExists(int id)
    {
        return context.Books.Any(e => e.Id == id);
    }
}