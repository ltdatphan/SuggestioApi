using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;
using SuggestioApi.Models;

namespace SuggestioApi.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDBContext _context;

    public UserRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<User?> ReadUserByUsernameAsync(string username)
    {
        var user = await _context.Users
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .Include(u => u.CuratedLists)
            .SingleOrDefaultAsync(u => u.UserName == username);

        if (user == null)
            return null;

        return user;
    }

    public async Task<User?> ReadUserByIdAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .Include(u => u.CuratedLists)
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return null;

        return user;
    }

    public async Task<bool> IsFollowingAsync(string currentUserId, string targetUserId)
    {
        var followModel = await _context.Follows.FirstOrDefaultAsync(f => f.CurrentUserId == currentUserId &&
                                                                          f.TargetUserId == targetUserId);

        if (followModel == null)
            return false;
        return true;
    }

    public async Task<PaginatedUserDto> SearchUsersAsync(string query, UserQueryObject userQueryObject)
    {
        query = query.ToLower();

        var userModels = _context.Users.AsQueryable();

        userModels = userModels.Where(u =>
            u.UserName!.ToLower().Contains(query) ||
            u.FirstName.ToLower().Contains(query) ||
            u.LastName.ToLower().Contains(query));

        if (!string.IsNullOrWhiteSpace(userQueryObject.SortBy))
        {
            //By Title
            if (userQueryObject.SortBy.Equals("Username", StringComparison.OrdinalIgnoreCase))
                userModels = userQueryObject.IsDescending
                    ? userModels.OrderByDescending(l => l.UserName)
                    : userModels.OrderBy(l => l.UserName);
            //By CreatedAt
            if (userQueryObject.SortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
                userModels = userQueryObject.IsDescending
                    ? userModels.OrderByDescending(l => l.FirstName)
                    : userModels.OrderBy(l => l.FirstName);
        }

        var skipNumber = (userQueryObject.PageNumber - 1) * userQueryObject.PageSize;
        var totalItems = await userModels.CountAsync();
        var paginatedUsers = await userModels.Skip(skipNumber).Take(userQueryObject.PageSize).ToListAsync();

        return new PaginatedUserDto
        {
            Users = paginatedUsers.Select(u => u.ToUserDto()).ToList(),
            PageNumber = userQueryObject.PageNumber,
            PageSize = userQueryObject.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedUserDto> ReadUserFollowersAsync(string userId, UserQueryObject userQueryObject)
    {
        var followersIds = await _context.Follows.Where(fl => fl.TargetUserId == userId).Select(fl => fl.CurrentUserId)
            .ToListAsync();

        if (followersIds.Count == 0)
            return new PaginatedUserDto
            {
                Users = [],
                PageNumber = userQueryObject.PageNumber,
                PageSize = userQueryObject.PageSize,
                TotalItems = 0
            };

        var userModels = _context.Users.Where(u => followersIds.Contains(u.Id)).AsQueryable();

        if (!string.IsNullOrWhiteSpace(userQueryObject.SortBy))
        {
            //By Title
            if (userQueryObject.SortBy.Equals("Username", StringComparison.OrdinalIgnoreCase))
                userModels = userQueryObject.IsDescending
                    ? userModels.OrderByDescending(l => l.UserName)
                    : userModels.OrderBy(l => l.UserName);
            //By CreatedAt
            if (userQueryObject.SortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
                userModels = userQueryObject.IsDescending
                    ? userModels.OrderByDescending(l => l.FirstName)
                    : userModels.OrderBy(l => l.FirstName);
        }

        var skipNumber = (userQueryObject.PageNumber - 1) * userQueryObject.PageSize;
        var totalItems = await userModels.CountAsync();
        var paginatedUsers = await userModels.Skip(skipNumber).Take(userQueryObject.PageSize).ToListAsync();

        return new PaginatedUserDto
        {
            Users = paginatedUsers.Select(u => u.ToUserDto()).ToList(),
            PageNumber = userQueryObject.PageNumber,
            PageSize = userQueryObject.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedUserDto> ReadUserFollowingsAsync(string userId, UserQueryObject userQueryObject)
    {
        var followingsIds = await _context.Follows.Where(fl => fl.CurrentUserId == userId).Select(fl => fl.TargetUserId)
            .ToListAsync();

        if (followingsIds.Count == 0)
            return new PaginatedUserDto
            {
                Users = [],
                PageNumber = userQueryObject.PageNumber,
                PageSize = userQueryObject.PageSize,
                TotalItems = 0
            };

        var userModels = _context.Users.Where(u => followingsIds.Contains(u.Id)).AsQueryable();

        if (!string.IsNullOrWhiteSpace(userQueryObject.SortBy))
        {
            //By Title
            if (userQueryObject.SortBy.Equals("Username", StringComparison.OrdinalIgnoreCase))
                userModels = userQueryObject.IsDescending
                    ? userModels.OrderByDescending(l => l.UserName)
                    : userModels.OrderBy(l => l.UserName);
            //By CreatedAt
            if (userQueryObject.SortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
                userModels = userQueryObject.IsDescending
                    ? userModels.OrderByDescending(l => l.FirstName)
                    : userModels.OrderBy(l => l.FirstName);
        }

        var skipNumber = (userQueryObject.PageNumber - 1) * userQueryObject.PageSize;
        var totalItems = await userModels.CountAsync();
        var paginatedUsers = await userModels.Skip(skipNumber).Take(userQueryObject.PageSize).ToListAsync();

        return new PaginatedUserDto
        {
            Users = paginatedUsers.Select(u => u.ToUserDto()).ToList(),
            PageNumber = userQueryObject.PageNumber,
            PageSize = userQueryObject.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<UserRelationship> ReadUsersRelationshipAsync(string targetUserId, string currentUserId)
    {
        if (targetUserId == currentUserId) return new UserRelationship();

        // Retrieve both relationships in a single query
        var follows = await _context.Follows
            .Where(f => (f.CurrentUserId == currentUserId && f.TargetUserId == targetUserId) ||
                        (f.CurrentUserId == targetUserId && f.TargetUserId == currentUserId))
            .ToListAsync();

        // Determine if the current user follows the target user and vice versa
        var isFollowingCurrentUser =
            follows.Any(f => f.CurrentUserId == targetUserId && f.TargetUserId == currentUserId);
        var isFollowedByCurrentUser =
            follows.Any(f => f.CurrentUserId == currentUserId && f.TargetUserId == targetUserId);

        var relationship = new UserRelationship(isFollowingCurrentUser, isFollowedByCurrentUser);

        return relationship;
    }

    public async Task<UserAndFollowStatus?> ReadUserAndFollowStatusAsync(string currentUserId, string targetUserId)
    {
        return await _context.Users
            .Where(u => u.Id == targetUserId)
            .Select(u => new UserAndFollowStatus
            {
                TargetUser = u,
                IsFollowing = _context.Follows
                    .Any(fl => fl.CurrentUserId == currentUserId && fl.TargetUserId == targetUserId)
            })
            .FirstOrDefaultAsync();
    }
}