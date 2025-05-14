using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readora.API.DtoModels;
using Readora.DataBase;
using Readora.Models;
using Readora.Models.Enums;
using Readora.Services;
using Readora.Services.Helpers;

namespace Readora.API.Controllers
{
    [Authorize(Roles = "Moderator,Admin")]
    [ApiController]
    [Route("api/moderation")]
    public class ModerationController(ReadoraDbContext context) : ControllerBase
    {
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingBooks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = context.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.ModerationRequest.Status == ModerationStatus.Pending)
                .OrderBy(b => b.UploadDate);

            var totalCount = await query.CountAsync();
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookModerationDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author.Username,
                    UploadDate = b.UploadDate,
                    Genres = b.Genres.Select(g => g.Name).ToList(),
                })
                .ToListAsync();

            return Ok(new { books, totalCount });
        }
        
        [HttpPost("approve/{bookId:int}")]
        public async Task<IActionResult> ApproveBook(
            int bookId,
            [FromBody] ModerationDecisionDto dto)
        {
            return await ProcessModeration(bookId, dto, ModerationStatus.Approved);
        }
        
        [HttpPost("reject/{bookId}")]
        public async Task<IActionResult> RejectBook(
            int bookId,
            [FromBody] ModerationDecisionDto dto)
        {
            return await ProcessModeration(bookId, dto, ModerationStatus.Rejected);
        }
        
        private async Task<IActionResult> ProcessModeration(
            int bookId, 
            ModerationDecisionDto dto,
            ModerationStatus status)
        {
            var book = await context.Books
                .Include(b => b.ModerationRequest)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                return NotFound("Book not found");
            }

            if (book.ModerationRequest.Status != ModerationStatus.Pending)
            {
                return BadRequest("Book already moderated");
            }

            var moderatorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (moderatorId is null)
            {
                return Unauthorized("User not found");
            }

            book.ModerationRequest.Status = status;
            book.ModerationRequest.ModeratorComment = dto.Comment;
            book.ModerationRequest.ModeratorId = Guid.Parse(moderatorId);
            book.ModerationRequest.DecisionDate = DateTime.UtcNow;

            await context.SaveChangesAsync();

            if (status == ModerationStatus.Approved)
            {
                await CreateBlockchainTransaction(book);
            }

            return Ok();
        }
        
        private async Task CreateBlockchainTransaction(Book book)
        {
            var service = new BlockchainService();
            var txHash = await service.AddBookAsync(book.Id, book.Title, book.FileHash);
            var transaction = new BlockchainTransaction
            {
                BookId = book.Id,
                TransactionHash = txHash ?? "",
                Timestamp = DateTime.UtcNow,
            };

            Console.WriteLine($"Tx Hash: {txHash}");
            
            await context.BlockchainTransactions.AddAsync(transaction);
            await context.SaveChangesAsync();
        }
    }
}
