using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SuggestioApi.Data;
using SuggestioApi.Policies.Requirements;

namespace SuggestioApi.Policies.Handlers;

public class ListOwnerHandler : AuthorizationHandler<ListOwnerRequirement>
{
    private readonly ApplicationDBContext _context;

    public ListOwnerHandler(ApplicationDBContext context)
    {
        _context = context;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ListOwnerRequirement requirement)
    {
        var httpContext = (HttpContext)context.Resource!;
        var listExist = httpContext.Request.RouteValues.TryGetValue("listId", out var listId);

        if (listExist)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var parsedListId = int.Parse(listId!.ToString()!);

            var list = _context.CuratedLists.FirstOrDefault(l => l.Id == parsedListId);

            if (list == null)
                return Task.CompletedTask;

            if (list.OwnerId == userId) context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}