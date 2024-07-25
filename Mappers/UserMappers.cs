using SuggestioApi.Dtos.User;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Models;

namespace SuggestioApi.Mappers;

public static class UserMappers
{
    public static UserDto ToUserDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImgUrl = user.ProfileImgUrl
        };
    }

    public static UserWithListDto ToUserWithListDto(this User user, bool isFollowing)
    {
        return new UserWithListDto
        {
            Id = user.Id,
            Username = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImgUrl = user.ProfileImgUrl,
            IsFollowing = isFollowing,
            UserLists = user.CuratedLists.Select(l => l.ToBasicListDto()).ToList()
        };
    }

    public static UserProfilePublicDto ToUserProfilePublicDto(this User user, UserRelationship relationship)
    {
        return new UserProfilePublicDto
        {
            Id = user.Id,
            Username = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImgUrl = user.ProfileImgUrl,
            IsFollowedByCurrentUser = relationship.IsFollowedByCurrentUser,
            IsFollowingCurrentUser = relationship.IsFollowingCurrentUser,
            FollowersCount = user.FollowersCount,
            FollowingsCount = user.FollowingsCount,
            ListCount = user.CuratedLists.Where(l => l.IsPublic).ToList().Count
            // UserLists = user.CuratedLists.Select(l => l.ToBasicListDto()).ToList(),
        };
    }

    public static UserProfileDto ToUserProfileDto(this User user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfileImgUrl = user.ProfileImgUrl,
            FollowersCount = user.FollowersCount,
            FollowingsCount = user.FollowingsCount,
            ListCount = user.CuratedLists.Count
            // UserLists = user.CuratedLists.Select(l => l.ToBasicListDto()).ToList(),
        };
    }
}