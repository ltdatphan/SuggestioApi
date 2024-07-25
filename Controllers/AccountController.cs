using System.Net.Sockets;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Dtos.Account;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Controllers;

[EnableRateLimiting("fixed")]
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly IRefreshTokenRepository _tokenRepo;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public AccountController(UserManager<User> userManager, ITokenService tokenService,
        SignInManager<User> signInManager, IRefreshTokenRepository tokenRepo)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _tokenRepo = tokenRepo;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequestDto loginDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

        if (user == null)
            return Unauthorized("User not found and/or password is incorrect");

        var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, true);

        if (!result.Succeeded)
            return Unauthorized("User not found and/or password is incorrect");

        var ipAddress = GetIpAddress();

        if (ipAddress == null)
            return StatusCode(500);

        var currentActiveTokens = await _tokenRepo.GetAllActiveTokens(user.Id);

        //TODO: Prevent token spamming
        if (currentActiveTokens.Count > 0)
        {
            // Console.WriteLine($"There are {currentActiveTokens.Count} active tokens");
            //Temp solution prevents token spamming
            // if (currentActiveTokens.Count > 10)
            // {
            //     return BadRequest("Active refresh token count exceeded limit of 10");
            // }
        }

        var refreshToken = await _tokenService.GenerateRefreshToken(ipAddress);

        var addResult = await _tokenRepo.AddRefreshToken(user.Id, refreshToken);

        if (addResult == null)
            return StatusCode(500);

        var jwtToken = _tokenService.CreateToken(user);

        setTokenCookie("accessToken", jwtToken);
        setTokenCookie("refreshToken", refreshToken.Token);

        var csrfToken = _tokenService.GenerateCsrfToken();
        var csrfCookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax
        };
        Response.Cookies.Append("CSRF-TOKEN", csrfToken, csrfCookieOptions);

        return Ok(new AuthResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            Username = user.UserName,
            Email = user.Email,
            ProfileImgUrl = user.ProfileImgUrl
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = registerDto.Username!,
                Email = registerDto.Email!,
                FirstName = registerDto.FirstName!,
                LastName = registerDto.LastName!
            };

            var createdUser = await _userManager.CreateAsync(user, registerDto.Password!);

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (roleResult.Succeeded)
                {
                    // NEW CODE:
                    var ipAddress = GetIpAddress();

                    if (ipAddress == null)
                        return StatusCode(500);

                    var refreshToken = await _tokenService.GenerateRefreshToken(ipAddress);

                    var addResult = await _tokenRepo.AddRefreshToken(user.Id, refreshToken);

                    if (addResult == null)
                        return StatusCode(500);

                    var jwtToken = _tokenService.CreateToken(user);
                    setTokenCookie("accessToken", jwtToken);
                    setTokenCookie("refreshToken", refreshToken.Token);
                    var csrfToken = _tokenService.GenerateCsrfToken();
                    var csrfCookieOptions = new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    };
                    Response.Cookies.Append("CSRF-TOKEN", csrfToken, csrfCookieOptions);

                    // =============================
                    return Ok(
                        new AuthResponseDto
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            Username = user.UserName,
                            Email = user.Email,
                            ProfileImgUrl = user.ProfileImgUrl
                        }
                    );
                }

                return StatusCode(500, roleResult.Errors);
            }

            return StatusCode(500, createdUser.Errors);
        }
        catch (Exception e)
        {
            return StatusCode(500, e);
        }
    }

    //TODO: fix this so that it works when access token expires
    [HttpPost("refresh-token")]
    //[ValidateAntiForgeryToken]
    public async Task<IActionResult> RefreshToken()
    {
        //Every time this route is use, we revoke the currently active refresh token
        //and generate a new refresh token
        //also known as: Refresh token rotation
        var refreshTokenFromCookie = Request.Cookies["refreshToken"];

        if (refreshTokenFromCookie == null)
            return Unauthorized("Authentication required");

        var user = await _tokenRepo.GetUserByRefreshToken(refreshTokenFromCookie);

        if (user == null)
            return BadRequest("Invalid token");

        var targetRefreshToken = user.RefreshTokens.Single(rf => rf.Token == refreshTokenFromCookie);

        //When user tries to use a revoked refresh token
        // Go down the chain and look for the latest refresh token created from the provided revoked token
        // Since the token/device is likely compromised 
        // Follow the chain to revoke the most recently created token
        var ipAddress = GetIpAddress();

        if (ipAddress == null)
            return StatusCode(500);

        if (targetRefreshToken.IsRevoked)
            await _tokenRepo
                .RevokeDescendantTokens(user, targetRefreshToken, ipAddress,
                    $"Attempted reuse of revoked ancestor token: {refreshTokenFromCookie}");

        //Refresh token no longer valid
        if (!targetRefreshToken.IsActive)
            return BadRequest("Token provided is no longer active");

        var newRefreshToken = await _tokenService.GenerateRefreshToken(ipAddress);

        await _tokenRepo.RotateToken(user, targetRefreshToken, ipAddress, newRefreshToken);

        //Generate new refresh token
        var jwtToken = _tokenService.CreateToken(user);
        setTokenCookie("accessToken", jwtToken);
        setTokenCookie("refreshToken", newRefreshToken.Token);

        var csrfToken = _tokenService.GenerateCsrfToken();
        var csrfCookieOptions = new CookieOptions
        {
            HttpOnly = false,
            Secure = true,
            SameSite = SameSiteMode.Lax
        };
        Response.Cookies.Append("CSRF-TOKEN", csrfToken, csrfCookieOptions);

        return Ok(
            new AuthResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Username = user.UserName,
                Email = user.Email,
                ProfileImgUrl = user.ProfileImgUrl
            }
        );
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken()
    {
        var refreshTokenFromCookie = Request.Cookies["refreshToken"];

        if (refreshTokenFromCookie == null)
            return Unauthorized();

        var user = await _tokenRepo.GetUserByRefreshToken(refreshTokenFromCookie);

        if (user == null)
            return BadRequest();

        var targetRefreshToken = user.RefreshTokens.Single(rf => rf.Token == refreshTokenFromCookie);

        if (targetRefreshToken == null || !targetRefreshToken.IsActive)
            return BadRequest("Invalid token");

        var ipAddress = GetIpAddress();

        if (ipAddress == null)
            return StatusCode(500);

        await _tokenRepo.RevokeToken(user, targetRefreshToken, ipAddress);

        Response.Cookies.Delete("refreshToken");
        Response.Cookies.Delete("CSRF-TOKEN");

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok("Token revoked");
    }

    //Private helper functions
    private void setTokenCookie(string key, string token)
    {
        // append cookie with refresh token to the http response
        var expires = key == "refreshToken"
            ? DateTime.UtcNow.AddDays(7)
            : DateTime.UtcNow.AddMinutes(15);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.None,
            Expires = expires
        };
        Response.Cookies.Append(key, token, cookieOptions);
    }

    private string? GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            // X-Forwarded-For can contain a comma-separated list of IPs, take the first one
            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault();
            if (!string.IsNullOrEmpty(ip)) return ip.Trim();
        }

        // Fall back to the remote IP address
        var remoteIpAddress = Request.HttpContext.Connection.RemoteIpAddress;
        if (remoteIpAddress != null)
        {
            // Handle both IPv4 and IPv6 addresses
            if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                return remoteIpAddress.MapToIPv4().ToString();

            return remoteIpAddress.ToString();
        }

        return null;
    }
}