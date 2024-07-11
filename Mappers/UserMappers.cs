using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.User;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Models;

namespace SuggestioApi.Mappers
{
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
                UserLists = user.CuratedLists.Select(l => l.ToBasicListDto()).ToList(),
            };
        }

        public static UserProfileDto ToUserProfileDto(this User user, UserRelationship relationship)
        {
            return new UserProfileDto
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
                UserLists = user.CuratedLists.Select(l => l.ToBasicListDto()).ToList(),
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
                IsFollowedByCurrentUser = null,
                IsFollowingCurrentUser = null,
                FollowersCount = user.FollowersCount,
                FollowingsCount = user.FollowingsCount,
                UserLists = user.CuratedLists.Select(l => l.ToBasicListDto()).ToList(),
            };
        }
    }
}