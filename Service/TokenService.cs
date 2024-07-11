using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly IRefreshTokenRepository _tokenRepo;
        public TokenService(IConfiguration config, IRefreshTokenRepository tokenRepo)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            _tokenRepo = tokenRepo;
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                //Jwt Token expires in 15 mins
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _config["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JWT:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken is JwtSecurityToken jwtSecurityToken && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch (Exception)
            {
                // Token validation failed
            }

            return null;
        }

        public async Task<RefreshToken> GenerateRefreshToken(string ipAddress)
        {
            var uniqueToken = await GetUniqueToken();
            var refreshToken = new RefreshToken
            {
                Token = uniqueToken,
                // token is valid for 7 days
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            //Local method to generate unique tokens by checking with db
            async Task<string> GetUniqueToken()
            {
                string token;
                bool isTokenUnique;

                do
                {
                    token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                    isTokenUnique = await _tokenRepo.IsTokenUnique(token);

                } while (!isTokenUnique);

                return token;
            }
        }
    }
}