using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Repository;

public class FollowRepository : IFollowRepository
{
    private readonly ApplicationDBContext _context;

    public FollowRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<Follow> FollowUserAsync(Follow followModel)
    {
        //Performed user id checks using GetUserByIdAsync
        await _context.Follows.AddAsync(followModel);
        await _context.SaveChangesAsync();
        return followModel;
    }

    public async Task<Follow?> UnfollowUserByIdAsync(string currentUserId, string targetUserId)
    {
        var followModel = await _context.Follows.FirstOrDefaultAsync(f =>
            f.CurrentUserId == currentUserId && f.TargetUserId == targetUserId);

        if (followModel == null)
            return null;

        _context.Follows.Remove(followModel);
        await _context.SaveChangesAsync();
        return followModel;
    }
}