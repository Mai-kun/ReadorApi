namespace Readora.API.DtoModels;

public class BookUploadDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int PublicationYear { get; set; }
    public string? Isbn { get; set; }
    public required List<int> Genres { get; set; }
    public required IFormFile BookFile { get; set; }
    public required IFormFile CoverImage { get; set; }
}