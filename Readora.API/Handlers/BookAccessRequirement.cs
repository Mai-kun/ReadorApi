using Microsoft.AspNetCore.Authorization;

namespace Readora.API.Handlers;

public class BookAccessRequirement : IAuthorizationRequirement
{
    public bool AllowPreview { get; }

    public BookAccessRequirement(bool allowPreview = true)
    {
        AllowPreview = allowPreview;
    }
}