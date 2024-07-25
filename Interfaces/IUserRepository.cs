using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces;

public interface IUserRepository
{
    Task<User?> ReadUserByUsernameAsync(string username);
    Task<User?> ReadUserByIdAsync(string userId);
    Task<bool> IsFollowingAsync(string targetUserId, string currentUserId);
    Task<UserRelationship> ReadUsersRelationshipAsync(string targetUserId, string currentUserId);
    Task<UserAndFollowStatus?> ReadUserAndFollowStatusAsync(string currentUserId, string targetUserId);
    Task<PaginatedUserDto> ReadUserFollowersAsync(string userId, UserQueryObject userQueryObject);
    Task<PaginatedUserDto> ReadUserFollowingsAsync(string userId, UserQueryObject userQueryObject);
    Task<PaginatedUserDto> SearchUsersAsync(string query, UserQueryObject userQueryObject);
}