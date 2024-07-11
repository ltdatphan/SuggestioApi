using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> IsTokenUnique(string token);
        Task<List<RefreshToken>> GetAllActiveTokens(string userId);
        Task<RefreshToken?> AddRefreshToken(string userId, RefreshToken refreshToken);
        Task<User?> GetUserByRefreshToken(string token);
        Task<RefreshToken?> RevokeToken(User user, RefreshToken token, string ipAddress);
        Task<RefreshToken?> RevokeDescendantTokens(User user, RefreshToken refreshToken, string ipAddress, string reason);
        Task<RefreshToken?> RotateToken(User user, RefreshToken token, string ipAddress, RefreshToken newToken);
    }
}