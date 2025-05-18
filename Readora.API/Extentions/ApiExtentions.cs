using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Readora.API.Handlers;
using Readora.Models;
using Readora.Models.Authentification;

namespace Readora.API.Extentions;

public static class ApiExtensions
{
    public static void AddApiAuthentication(this IServiceCollection services, IConfigurationSection jwtConfig)
    {
        services.Configure<JwtOptions>(jwtConfig);

        var jwtOptions = jwtConfig.Get<JwtOptions>();
        if (string.IsNullOrEmpty(jwtOptions?.Key))
        {
            throw new ArgumentException("JWT Key is not configured");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)),
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[nameof(StringLiterals.TastyCookies)];
                        
                        return Task.CompletedTask;
                    },
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("ModeratorOnly", policy => 
                policy.RequireRole("Moderator", "Admin"))
            .AddPolicy("BookAccess", policy => 
                policy.Requirements.Add(new BookAccessRequirement(true)));
        
        services.AddSingleton<IAuthorizationHandler, BookAccessHandler>();
        services.AddAuthorization();
    }
}