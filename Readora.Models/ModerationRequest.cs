using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readora.Models.Abstractions;
using Readora.Models.Enums;

namespace Readora.Models;

[Table("ModerationRequests")]
public class ModerationRequest : IEntityWithGuidId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required int BookId { get; set; }
    
    public required Book Book { get; set; }

    public Guid? ModeratorId { get; set; }
    public User? Moderator { get; set; }

    public ModerationStatus Status { get; set; }

    [MaxLength(3000)]
    public string? ModeratorComment { get; set; }

    public DateTime RequestDate { get; set; }
    public DateTime? DecisionDate { get; set; }
}