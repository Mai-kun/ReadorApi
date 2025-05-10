namespace Readora.Models;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public int BookId { get; set; }
    public Book? Book { get; set; }
    
    public Guid UserId { get; set; }
    public User? User { get; set; }
}