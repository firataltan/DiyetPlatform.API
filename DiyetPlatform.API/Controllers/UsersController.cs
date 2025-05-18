using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.Application.DTOs.User;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;

namespace DiyetPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var users = await _userService.GetUsersAsync(userParams);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(user);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            return Ok(user);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileUpdateDto profileDto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _userService.UpdateUserAsync(userId, profileDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("follow/{targetId}")]
        public async Task<IActionResult> FollowUser(int targetId)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            if (userId == targetId)
                return BadRequest("Kendinizi takip edemezsiniz.");

            var result = await _userService.FollowUserAsync(userId, targetId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("unfollow/{targetId}")]
        public async Task<IActionResult> UnfollowUser(int targetId)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _userService.UnfollowUserAsync(userId, targetId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("{id}/followers")]
        public async Task<IActionResult> GetUserFollowers(int id, [FromQuery] UserParams userParams)
        {
            var followers = await _userService.GetFollowersAsync(id, userParams);
            return Ok(followers);
        }

        [HttpGet("{id}/following")]
        public async Task<IActionResult> GetUserFollowing(int id, [FromQuery] UserParams userParams)
        {
            var following = await _userService.GetFollowingAsync(id, userParams);
            return Ok(following);
        }

        [Authorize]
        [HttpDelete("account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _userService.DeleteUserAsync(userId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}