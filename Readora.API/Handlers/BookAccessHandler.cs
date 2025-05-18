using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Readora.Models;
using Readora.Models.Enums;

namespace Readora.API.Handlers;

public class BookAccessHandler : AuthorizationHandler<BookAccessRequirement, Book>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BookAccessRequirement requirement,
        Book resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        switch (resource.ModerationRequest.Status)
        {
            case ModerationStatus.Approved:
                context.Succeed(requirement);
                break;

            case ModerationStatus.Pending:
            case ModerationStatus.Rejected:
                if (userId is not null)
                {
                    var isAuthor = resource.AuthorId == Guid.Parse(userId);
                    var isModerator = context.User.IsInRole("Moderator");
                    var isAdmin = context.User.IsInRole("Admin");
                    if (isAuthor || isModerator || isAdmin)
                        context.Succeed(requirement);
                    else
                        context.Fail();
                }
                break;
            default:
                context.Fail();
                break;
        }

        return Task.CompletedTask;
    }
}