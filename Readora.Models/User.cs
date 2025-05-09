using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readora.Models.Abstractions;

namespace Readora.Models;

[Table("Users")]
public class User : IEntityWithGuidId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(100)]
    public required string Username { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [ForeignKey("RoleId")]
    public required Role Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(512)]
    public string? PublicKey { get; set; }

    public Guid CredentialId { get; set; }
    
    public required UserCredential Credential { get; set; }

    public ICollection<Book>? Books { get; set; }
}