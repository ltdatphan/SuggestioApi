namespace SuggestioApi.Helpers.CustomReturns;

public class UserRelationship
{
    public bool? IsFollowedByCurrentUser = false;
    public bool? IsFollowingCurrentUser = false;

    public UserRelationship(bool IsFollowingCurrentUser = false, bool IsFollowedByCurrentUser = false)
    {
        this.IsFollowingCurrentUser = IsFollowingCurrentUser;
        this.IsFollowedByCurrentUser = IsFollowedByCurrentUser;
    }
}