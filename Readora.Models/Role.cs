using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readora.Models.Abstractions;

namespace Readora.Models;

[Table("Roles")]
public class Role : IEntityWithIntId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(20)]
    public required string Name { get; set; }
    
    [MaxLength(200)]
    public required string Description { get; set; }

    public required ICollection<User> Users { get; set; } = [];
}