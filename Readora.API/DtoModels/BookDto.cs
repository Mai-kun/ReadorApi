namespace Readora.API.DtoModels;

public class BookDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public Guid AuthorId { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
    public required string CoverUrl { get; set; }
    public required List<string> Genres { get; set; } = [];
    public DateTime UploadDate { get; set; }
    public required int PublicationYear { get; set; } = 0;
    public string? Isbn { get; set; }
    public string? Status { get; set; }
}