using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuggestioApi.Dtos.Follow;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;

namespace SuggestioApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // [Authorize]
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetUserById([FromRoute] string id)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //     if (currentUserId == null)
        //         return Unauthorized("You do not have permission to view this user.");

        //     var user = await _userRepo.GetUserByIdAsync(id);

        //     if (user == null)
        //         return NotFound("User cannot be found.");

        //     return Ok(user.ToUserDto());
        // }

        [Authorize]
        [HttpGet("{username}/profile")]
        public async Task<IActionResult> GetUserProfileByUsername([FromRoute] string username)
        {
            if (!ValidateUsername(username, out var validationErrorMessage))
            {
                ModelState.AddModelError("Username", validationErrorMessage);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            username = username.ToLower();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to view this user.");

            var user = await _userRepo.GetUserProfileByUsername(username);

            if (user == null)
                return NotFound("User cannot be found.");

            if (user.Id == currentUserId)
                return Ok(user.ToUserProfileDto());

            var relationship = await _userRepo.GetRelationship(user.Id, currentUserId);
            return Ok(user.ToUserProfileDto(relationship));
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to view this user.");

            var user = await _userRepo.GetUserProfileById(currentUserId, showAllList: true);

            if (user == null)
                return NotFound("User cannot be found.");
            return Ok(user.ToUserProfileDto());
        }

        // [Authorize]
        // [HttpGet("{id}/details")]
        // public async Task<IActionResult> GetUserDetailsById([FromRoute] string id)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //     if (currentUserId == null)
        //         return Unauthorized("You do not have permission to view this user.");

        //     // if (currentUserId == id)
        //     //     return BadRequest("You cannot follow yourself");

        //     var user = await _userRepo.GetUserDetailsByIdAsync(id);

        //     if (user == null)
        //         return NotFound("User cannot be found.");

        //     var isFollowing = await _userRepo.IsFollowingAsync(currentUserId, id);

        //     return Ok(user.ToUserWithListDto(isFollowing));
        // }


        [Authorize]
        [HttpPost("{targetUserId}/follow")]
        public async Task<IActionResult> FollowUserById([FromRoute] string targetUserId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to follow this user.");

            if (currentUserId == targetUserId)
                return BadRequest("You cannot follow yourself");

            var targetUserModel = await _userRepo.GetUserByIdAsync(targetUserId);
            if (targetUserModel == null)
                return NotFound("The user you want to follow cannot be found");

            // Optional step to verify with DB
            var currentUserModel = await _userRepo.GetUserByIdAsync(currentUserId);
            if (currentUserModel == null)
                return Unauthorized("Your user id is invalid");

            // Verify if the user already followed
            var isFollowing = await _userRepo.IsFollowingAsync(currentUserId, targetUserId);
            if (isFollowing == true)
                return BadRequest("You already followed this user");

            var followRequestDto = new FollowRequestDto
            {
                TargetUserId = targetUserModel.Id,
                CurrentUserId = currentUserModel.Id,
            };

            var followModel = await _userRepo.FollowUserAsync(followRequestDto.ToFollowFromCreateDto());

            return Ok(followModel.ToFollowDto());
        }

        [Authorize]
        [HttpDelete("{targetUserId}/unfollow")]
        public async Task<IActionResult> UnfollowUserById([FromRoute] string targetUserId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to unfollow this user.");

            if (currentUserId == targetUserId)
                return BadRequest("You cannot unfollow yourself");

            var followModel = await _userRepo.UnfollowUserByIdAsync(currentUserId, targetUserId);

            if (followModel == null)
                return NotFound("User not found");

            return NoContent();
        }

        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUser([FromQuery] string query)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to unfollow this user.");

            var userModels = await _userRepo.SearchUsers(query);

            if (userModels.Count == 0)
                return Ok();

            return Ok(userModels.Select(u => u.ToUserDto()));
        }

        [Authorize]
        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers([FromRoute] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to view this.");

            var followers = await _userRepo.GetFollowers(userId);

            if (followers.Count == 0)
                return NotFound("No followers found");

            return Ok(followers.Select(f => f.ToUserDto()));
        }

        [Authorize]
        [HttpGet("{userId}/followings")]
        public async Task<IActionResult> GetFollowings([FromRoute] string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to view this.");

            var followers = await _userRepo.GetFollowings(userId);

            if (followers.Count == 0)
                return NotFound("No followings found");

            return Ok(followers.Select(f => f.ToUserDto()));
        }

        [Authorize]
        [HttpGet("feed")]
        public async Task<IActionResult> GetFollowingsLists()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null)
                return Unauthorized("You do not have permission to view this.");

            var lists = await _userRepo.GetFollowingsLists(currentUserId);

            // if (lists.Count == 0)
            //     return NotFound("No lists found");

            return Ok(lists.Select(f => f.ToCuratedListWithUserDto()));
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
                errorMessage = "Username must be between 1 and 30 characters long and can only contain letters, numbers, periods, and underscores. It cannot have consecutive periods or underscores.";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

    }
}