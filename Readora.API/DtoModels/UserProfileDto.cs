namespace Readora.API.DtoModels;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PublicKey { get; set; }
    public int TotalBooks { get; set; }
    public DateTime? LastPasswordChange { get; set; } // Из Credential
    public List<BookDto> Books { get; set; }
    public string Role { get; set; }
}