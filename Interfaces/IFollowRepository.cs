using SuggestioApi.Models;

namespace SuggestioApi.Interfaces;

public interface IFollowRepository
{
    Task<Follow> FollowUserAsync(Follow followModel);
    Task<Follow?> UnfollowUserByIdAsync(string currentUserId, string targetUserId);
}