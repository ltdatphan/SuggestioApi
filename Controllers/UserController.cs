using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SuggestioApi.Dtos.CuratedList;
using SuggestioApi.Dtos.Follow;
using SuggestioApi.Helpers;
using SuggestioApi.Helpers.CustomReturns;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;

namespace SuggestioApi.Controllers;

[EnableRateLimiting("fixed")]
[Route("api")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ICuratedListRepository _curatedListRepo;
    private readonly IFollowRepository _followRepo;
    private readonly IUserRepository _userRepo;

    public UserController(IUserRepository userRepo, ICuratedListRepository curatedListRepo,
        IFollowRepository followRepo)
    {
        _userRepo = userRepo;
        _curatedListRepo = curatedListRepo;
        _followRepo = followRepo;
    }

    [Authorize]
    [HttpGet("me/lists")]
    public async Task<IActionResult> GetCurrentUserLists([FromQuery] ListQueryObject listsQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("You do not have permission to view this list.");

        var paginatedListsDto = await _curatedListRepo.ReadUserListsPaginatedAsync(userId, listsQueryObject);

        return Ok(paginatedListsDto);
    }

    [Authorize]
    //[ValidateAntiForgeryToken]
    [HttpPost("me/lists")]
    public async Task<IActionResult> CreateList([FromBody] CreateCuratedListRequestDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("You do not have permission to create a list.");

        var listModel = createDto.ToListFromCreateDto(userId);
        var createModel = await _curatedListRepo.CreateListAsync(listModel);
        return Ok(createModel.ToListDto());
    }

    [Authorize]
    [HttpGet("me/profile")]
    public async Task<IActionResult> GetCurrentUserProfile()
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
            return Unauthorized("You do not have permission to view this user.");

        var userModel = await _userRepo.ReadUserByIdAsync(currentUserId);

        if (userModel == null)
            return NotFound("User not found.");

        return Ok(userModel.ToUserProfileDto());
    }

    [Authorize]
    [HttpGet("users/{username}/profile")]
    public async Task<IActionResult> GetUserProfileByUsername([FromRoute] string username)
    {
        if (!ValidateUsername(username, out var validationErrorMessage))
            ModelState.AddModelError("Username", validationErrorMessage);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        username = username.ToLower();

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
            return Unauthorized("You do not have permission to view this user.");

        var userModel = await _userRepo.ReadUserByUsernameAsync(username);

        if (userModel == null)
            return NotFound("User cannot be found.");

        // If its the current user
        if (userModel.Id == currentUserId)
            return Ok(userModel.ToUserProfilePublicDto(new UserRelationship
                { IsFollowedByCurrentUser = null, IsFollowingCurrentUser = null }));

        // For other users, show relationship
        var relationship = await _userRepo.ReadUsersRelationshipAsync(userModel.Id, currentUserId);
        return Ok(userModel.ToUserProfilePublicDto(relationship));
    }

    [Authorize]
    [HttpGet("me/following/lists")]
    public async Task<IActionResult> GetFollowingsLists([FromQuery] ListQueryObject listsQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
            return Unauthorized("You do not have permission to view this.");

        var paginatedListsPublicDto = await _curatedListRepo.ReadFollowingsListsAsync(currentUserId, listsQueryObject);

        return Ok(paginatedListsPublicDto);
    }

    [Authorize]
    [HttpGet("users/{username}/lists")]
    public async Task<IActionResult> GetPublicListsFromProfile([FromRoute] string username,
        [FromQuery] ListQueryObject listsQueryObject)
    {
        if (!ValidateUsername(username, out var validationErrorMessage))
            ModelState.AddModelError("Username", validationErrorMessage);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        username = username.ToLower();

        var userModel = await _userRepo.ReadUserByUsernameAsync(username);

        if (userModel == null)
            return NotFound("User cannot be found.");

        var paginatedListsPublicDto = await _curatedListRepo.ReadUserPublicListsAsync(userModel.Id, listsQueryObject);

        return Ok(paginatedListsPublicDto);
    }

    [Authorize]
    //[ValidateAntiForgeryToken]
    [HttpPost("users/{targetUserId}/follow")]
    public async Task<IActionResult> FollowUserById([FromRoute] string targetUserId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
            return Unauthorized("You do not have permission to follow this user.");

        if (currentUserId == targetUserId)
            return BadRequest("You cannot follow yourself.");

        var userAndFollowStatus = await _userRepo.ReadUserAndFollowStatusAsync(currentUserId, targetUserId);
        if (userAndFollowStatus == null)
            return NotFound("The user you want to follow cannot be found.");

        // Verify if the user already followed
        if (userAndFollowStatus.IsFollowing)
            return BadRequest("You already followed this user.");

        var followRequestDto = new FollowRequestDto
        {
            TargetUserId = userAndFollowStatus.TargetUser.Id,
            CurrentUserId = currentUserId
        };

        var followModel = await _followRepo.FollowUserAsync(followRequestDto.ToFollowFromCreateDto());

        return Ok(followModel.ToFollowDto());
    }

    [Authorize]
    //[ValidateAntiForgeryToken]
    [HttpDelete("users/{targetUserId}/unfollow")]
    public async Task<IActionResult> UnfollowUserById([FromRoute] string targetUserId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
            return Unauthorized("You do not have permission to unfollow this user.");

        if (currentUserId == targetUserId)
            return BadRequest("You cannot unfollow yourself");

        var unfollowModel = await _followRepo.UnfollowUserByIdAsync(currentUserId, targetUserId);

        if (unfollowModel == null)
            return NotFound("The user you want to follow cannot be found.");

        return NoContent();
    }

    [Authorize]
    [HttpGet("users/search")]
    public async Task<IActionResult> SearchUser([FromQuery] UsersSearchQueryObject searchQueryObject,
        [FromQuery] UserQueryObject userQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var paginatedUserDto = await _userRepo.SearchUsersAsync(searchQueryObject.Query, userQueryObject);

        return Ok(paginatedUserDto);
    }

    [Authorize]
    [HttpGet("users/{userId}/followers")]
    public async Task<IActionResult> GetFollowers([FromRoute] string userId,
        [FromQuery] UserQueryObject userQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var paginatedUserDto = await _userRepo.ReadUserFollowersAsync(userId, userQueryObject);

        return Ok(paginatedUserDto);
    }

    [Authorize]
    [HttpGet("users/{userId}/followings")]
    public async Task<IActionResult> GetFollowings([FromRoute] string userId,
        [FromQuery] UserQueryObject userQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var paginatedUserDto = await _userRepo.ReadUserFollowingsAsync(userId, userQueryObject);

        return Ok(paginatedUserDto);
    }

    private bool ValidateUsername(string username, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            errorMessage = "Username cannot be empty or whitespace.";
            return false;
        }

        if (username.Length < 3 || username.Length > 20)
        {
            errorMessage = "Username must be between 3 and 20 characters long.";
            return false;
        }

        if (!Regex.IsMatch(username, @"^(?!.*[\._]{2})[a-zA-Z0-9._]{1,30}$"))
        {
            errorMessage =
                "Username must be between 1 and 30 characters long and can only contain letters, numbers, periods, and underscores. It cannot have consecutive periods or underscores.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}