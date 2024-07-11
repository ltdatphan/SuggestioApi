using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        Task<RefreshToken> GenerateRefreshToken(string ipAddress);
    }
}