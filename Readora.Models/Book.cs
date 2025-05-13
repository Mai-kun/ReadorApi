using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readora.Models.Abstractions;

namespace Readora.Models;

[Table("Books")]
public class Book : IEntityWithIntId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public required Guid AuthorId { get; set; }
    public User? Author { get; set; }

    [MaxLength(200)]
    public required string Title { get; set; }

    [MaxLength(3000)]
    public string? Description { get; set; }

    [MaxLength(255)]
    public required string FileHash { get; set; }

    [MaxLength(512)]
    public required string FilePath { get; set; }

    [MaxLength(512)]
    public required string CoverImagePath { get; set; }

    [MaxLength(25)]
    public required int Status { get; set; }

    public required int PublicationYear { get; set; } = 0;

    public string? Isbn { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public ICollection<Genre> Genres { get; set; } = [];

    public ModerationRequest? ModerationRequest { get; set; }

    public BlockchainTransaction? BlockchainTransaction { get; set; }
    
    public string? Content { get; set; }
    public List<Comment> Comments { get; set; } = [];
}