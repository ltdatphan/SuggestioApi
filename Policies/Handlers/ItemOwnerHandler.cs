using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Policies.Requirements;

namespace SuggestioApi.Policies.Handlers;

public class ItemOwnerHandler : AuthorizationHandler<ItemOwnerRequirement>
{
    private readonly ApplicationDBContext _context;

    public ItemOwnerHandler(ApplicationDBContext context)
    {
        _context = context;
        ;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ItemOwnerRequirement requirement)
    {
        var httpContext = (HttpContext)context.Resource!;
        var itemExist = httpContext.Request.RouteValues.TryGetValue("itemId", out var itemId);

        if (itemExist)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var parsedItemId = int.Parse(itemId!.ToString()!);

            var item = _context.Items.Include(i => i.CuratedList).FirstOrDefault(i => i.Id == parsedItemId);

            if (item == null)
                return Task.CompletedTask;

            if (item.CuratedList.OwnerId == userId) context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}