using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Readora.Models.Abstractions;

namespace Readora.Models;

[Table("BlockchainTransactions")]
public class BlockchainTransaction : IEntityWithGuidId
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required int BookId { get; set; }
    
    public required Book Book { get; set; }

    [MaxLength(255)]
    public required string TransactionHash { get; set; }

    public required long BlockNumber { get; set; }

    public required DateTime Timestamp { get; set; }
}