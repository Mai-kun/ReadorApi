namespace Readora.API.DtoModels;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string CoverUrl { get; set; }
    public List<string> Genres { get; set; }
    public DateTime UploadDate { get; set; }
}