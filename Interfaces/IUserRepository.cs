using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.Follow;
using SuggestioApi.Dtos.User;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserProfileByUsername(string username, bool showAllList = false);
        Task<User?> GetUserProfileById(string userId, bool showAllList = false);
        Task<User?> GetUserDetailsByIdAsync(string userId);
        Task<Follow> FollowUserAsync(Follow followModel);
        Task<Follow?> UnfollowUserByIdAsync(string currentUserId, string targetUserId);
        Task<bool> IsFollowingAsync(string targetUserId, string currentUserId);
        Task<UserRelationship> GetRelationship(string targetUserId, string currentUserId);
        Task<List<User>> GetFollowers(string userId);
        Task<List<User>> GetFollowings(string userId);
        Task<List<CuratedList>> GetFollowingsLists(string userId);
        Task<List<User>> SearchUsers(string query);
    }
}