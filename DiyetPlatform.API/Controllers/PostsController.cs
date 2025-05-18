using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Core.Common;

namespace DiyetPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts([FromQuery] DiyetPlatform.Core.Common.PostParams postParams)
        {
            var posts = await _postService.GetPostsAsync(postParams);
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postService.GetPostByIdAsync(id);

            if (post == null)
                return NotFound("Gönderi bulunamadı.");

            return Ok(post);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPosts(int userId, [FromQuery] DiyetPlatform.Core.Common.PostParams postParams)
        {
            var posts = await _postService.GetUserPostsAsync(userId, postParams);
            return Ok(posts);
        }

        [Authorize]
        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed([FromQuery] DiyetPlatform.Core.Common.PostParams postParams)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var posts = await _postService.GetUserFeedAsync(userId, postParams);
            return Ok(posts);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateDto postDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var response = await _postService.CreatePostAsync(userId, postDto);

            if (!response.Success)
                return BadRequest(response.Message);

            return CreatedAtAction(nameof(GetPost), new { id = response.Data.Id }, response.Data);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromForm] PostCreateDto postDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _postService.UpdatePostAsync(userId, id, postDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _postService.DeletePostAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePost(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _postService.LikePostAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/unlike")]
        public async Task<IActionResult> UnlikePost(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _postService.UnlikePostAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/comment")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CommentCreateDto commentDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var result = await _postService.AddCommentAsync(userId, id, commentDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPosts([FromQuery] string query, [FromQuery] DiyetPlatform.Core.Common.PostParams postParams)
        {
            var posts = await _postService.SearchPostsAsync(query, postParams);
            return Ok(posts);
        }
    }
}