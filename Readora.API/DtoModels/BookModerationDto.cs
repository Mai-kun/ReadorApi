namespace Readora.API.DtoModels;

public class BookModerationDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime UploadDate { get; set; }
    public List<string> Genres { get; set; }
}