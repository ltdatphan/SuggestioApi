using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDBContext _context;
        public RefreshTokenRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> AddRefreshToken(string userId, RefreshToken refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            user.RefreshTokens.Add(refreshToken);

            //Remove all NON-active and expired tokens
            removeOldTokens(user);

            await _context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<List<RefreshToken>> GetAllActiveTokens(string userId)
        {
            var userModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (userModel == null)
                return [];

            var activeTokens = userModel.RefreshTokens.Where(t => t.IsActive).ToList();
            return activeTokens;
        }

        public async Task<User?> GetUserByRefreshToken(string token)
        {
            var userModel = await _context.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == token));

            if (userModel == null)
                return null;

            return userModel;
        }

        public async Task<bool> IsTokenUnique(string token)
        {
            return !await _context.Users.AnyAsync(u => u.RefreshTokens.Any(t => t.Token == token));
        }

        public async Task<RefreshToken?> RevokeToken(User user, RefreshToken token, string ipAddress)
        {
            revokeRefreshToken(token, ipAddress, "Revoked without replacement");
            _context.Update(user);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken?> RevokeDescendantTokens(User user, RefreshToken token, string ipAddress, string reason)
        {
            revokeDescendantRefreshTokens(user, token, ipAddress, reason);
            _context.Update(user);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken?> RotateToken(User user, RefreshToken token, string ipAddress, RefreshToken newToken)
        {
            rotateRefreshToken(token, ipAddress, newToken.Token);
            user.RefreshTokens.Add(newToken);

            removeOldTokens(user);

            _context.Update(user);
            await _context.SaveChangesAsync();
            return newToken;
        }

        private void removeOldTokens(User user)
        {
            //Only save tokens for 2 days
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive && x.Created.AddDays(2) <= DateTime.UtcNow);
        }

        private void rotateRefreshToken(RefreshToken refreshToken, string ipAddress, string newRefreshToken)
        {
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken);
        }

        private void revokeDescendantRefreshTokens(User user, RefreshToken refreshToken, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                    revokeRefreshToken(childToken, ipAddress, reason);
                else
                    revokeDescendantRefreshTokens(user, childToken, ipAddress, reason);
            }
        }

        private void revokeRefreshToken(RefreshToken token, string ipAddress, string? reason = null, string? replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason != null ? reason : string.Empty;
            token.ReplacedByToken = replacedByToken != null ? replacedByToken : string.Empty;
        }


    }
}