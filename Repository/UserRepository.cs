using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.Follow;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;
using SuggestioApi.Models;

namespace SuggestioApi.Repository
{

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;
        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return user;
        }

        public async Task<User?> GetUserDetailsByIdAsync(string userId)
        {
            var user = await _context.Users.Include(u => u.CuratedLists).SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return user;
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

        public async Task<bool> IsFollowingAsync(string currentUserId, string targetUserId)
        {
            var followModel = await _context.Follows.
                FirstOrDefaultAsync(f => f.CurrentUserId == currentUserId &&
                                        f.TargetUserId == targetUserId);

            if (followModel == null)
                return false;
            else
                return true;
        }

        public async Task<List<User>> SearchUsers(string query)
        {
            query = query.ToLower();
            var userModels = await _context.Users.Where(u =>
                 u.UserName.ToLower().Contains(query) ||
                 u.FirstName.ToLower().Contains(query) ||
                 u.LastName.ToLower().Contains(query)).
                 ToListAsync();

            return userModels;
        }

        public async Task<List<User>> GetFollowers(string userId)
        {
            var followersIds = await _context.Follows.Where(fl => fl.TargetUserId == userId).Select(fl => fl.CurrentUserId).ToListAsync();

            if (followersIds.Count == 0)
                return new List<User>();

            var userModels = await _context.Users.Where(u => followersIds.Contains(u.Id)).ToListAsync();

            // var userModels = await _context.Follows.Where(fl => fl.TargetUserId == userId)
            // .Join(_context.Users, follow => follow.CurrentUserId, user => user.Id, (follow, user) => user).ToListAsync();

            // if (userModels.Count == 0)
            //     return new List<User>();

            return userModels;
        }

        public async Task<List<User>> GetFollowings(string userId)
        {
            // var userModels = await _context.Follows.Where(fl => fl.CurrentUserId == userId)
            // .Join(_context.Users, follow => follow.TargetUserId, user => user.Id, (follow, user) => user).ToListAsync();

            // if (userModels.Count == 0)
            //     return new List<User>();

            var followingsIds = await _context.Follows.Where(fl => fl.CurrentUserId == userId).Select(fl => fl.TargetUserId).ToListAsync();

            if (followingsIds.Count == 0)
                return new List<User>();

            var userModels = await _context.Users.Where(u => followingsIds.Contains(u.Id)).ToListAsync();

            return userModels;
        }

        public async Task<List<CuratedList>> GetFollowingsLists(string userId)
        {
            var targetUserIds = await _context.Follows
                .Where(fl => fl.CurrentUserId == userId)
                .Select(fl => fl.TargetUserId)
                .ToListAsync();

            if (targetUserIds.Count == 0)
            {
                return new List<CuratedList>();
            }

            var listModels = await _context.CuratedLists
                .Where(curList => targetUserIds.Contains(curList.OwnerId) && curList.IsPublic)
                .Include(curList => curList.User)
                .OrderBy(l => l.CreatedAt)
                .ToListAsync();

            return listModels;
        }

        public async Task<User?> GetUserProfileByUsername(string username, bool showAllList = true)
        {
            var user = await _context.Users
                .Include(u => u.CuratedLists)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                return null;
            if (!showAllList)
                user.CuratedLists = user.CuratedLists.Where(cl => cl.IsPublic == true).ToList();

            return user;
        }

        public async Task<User?> GetUserProfileById(string userId, bool showAllList = true)
        {
            var user = await _context.Users
            .Include(u => u.CuratedLists)
            .Include(u => u.Followers)
            .Include(u => u.Following)
            .SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            if (!showAllList)
                user.CuratedLists = user.CuratedLists.Where(cl => cl.IsPublic == true).ToList();

            return user;
        }

        public async Task<UserRelationship> GetRelationship(string targetUserId, string currentUserId)
        {
            if (targetUserId == currentUserId)
            {
                return new UserRelationship();
            }

            // Retrieve both relationships in a single query
            var follows = await _context.Follows
                .Where(f => (f.CurrentUserId == currentUserId && f.TargetUserId == targetUserId) ||
                            (f.CurrentUserId == targetUserId && f.TargetUserId == currentUserId))
                .ToListAsync();

            // Determine if the current user follows the target user and vice versa
            var isFollowingCurrentUser = follows.Any(f => f.CurrentUserId == targetUserId && f.TargetUserId == currentUserId);
            var isFollowedByCurrentUser = follows.Any(f => f.CurrentUserId == currentUserId && f.TargetUserId == targetUserId);

            var relationship = new UserRelationship(IsFollowingCurrentUser: isFollowingCurrentUser, IsFollowedByCurrentUser: isFollowedByCurrentUser);

            return relationship;
        }
    }
}