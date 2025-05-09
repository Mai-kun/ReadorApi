using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readora.Models.Abstractions;

namespace Readora.Models;

[Table("UserCredentials")]
public class UserCredential : IEntityWithGuidId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public User? User { get; set; }

    [MaxLength(255)]
    public required string PasswordHash { get; set; }
}