namespace Readora.Models.Authentification;

public class JwtOptions
{
    public required string Key { get; set; }         // Секретный ключ для подписи токена
    public int AccessTokenLifetimeYears { get; set; } // Время жизни access-токена
    public string? Issuer { get; set; }         // Идентификатор издателя токена
    public string? Audience { get; set; }         // Идентификатор потребителя токена
}