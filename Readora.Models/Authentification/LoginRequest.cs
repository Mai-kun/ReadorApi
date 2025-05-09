using System.ComponentModel.DataAnnotations;

namespace Readora.Models.Authentification;

public class LoginRequest
{
    [EmailAddress]
    public required string Email { get; set; }

    public required string Password { get; set; }
}