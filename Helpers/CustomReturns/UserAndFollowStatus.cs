using SuggestioApi.Models;

namespace SuggestioApi.Helpers.CustomReturns;

public class UserAndFollowStatus
{
    public User TargetUser { get; set; } = null!;
    public bool IsFollowing { get; set; }
}