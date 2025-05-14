using System.ComponentModel.DataAnnotations;

namespace Readora.API.DtoModels;

public class ModerationDecisionDto
{
    [StringLength(1000)]
    public string? Comment { get; set; }
}