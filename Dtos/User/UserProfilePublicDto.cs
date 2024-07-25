namespace SuggestioApi.Dtos.User;

public class UserProfilePublicDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImgUrl { get; set; }

    // Derived prop. Depending on the user viewing this users profile.
    public bool? IsFollowingCurrentUser { get; set; } = false;
    public bool? IsFollowedByCurrentUser { get; set; } = false;
    public int FollowersCount { get; set; } = 0;
    public int FollowingsCount { get; set; } = 0;
    public int ListCount { get; set; } = 0;

    //Users lists
    // public List<BasicCuratedListDto> UserLists { get; set; } = new List<BasicCuratedListDto>();
}