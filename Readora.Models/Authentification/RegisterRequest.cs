using System.ComponentModel.DataAnnotations;

namespace Readora.Models.Authentification;

public class RegisterRequest
{
    [EmailAddress]
    public required string Email { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }

    public required string Username { get; set; }
}