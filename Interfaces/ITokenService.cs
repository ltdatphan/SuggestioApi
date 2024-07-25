using SuggestioApi.Models;

namespace SuggestioApi.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
    Task<RefreshToken> GenerateRefreshToken(string ipAddress);
    string GenerateCsrfToken();
}