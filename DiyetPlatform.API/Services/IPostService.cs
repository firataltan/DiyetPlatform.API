using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
{
    public interface IPostService
    {
        Task<PagedList<PostResponseDto>> GetPostsAsync(PostParams postParams);
        Task<PostResponseDto> GetPostByIdAsync(int id);
        Task<PagedList<PostResponseDto>> GetUserPostsAsync(int userId, PostParams postParams);
        Task<PagedList<PostResponseDto>> GetUserFeedAsync(int userId, PostParams postParams);
        Task<PostResponseDto> CreatePostAsync(int userId, PostCreateDto postDto);
        Task<ServiceResponse<PostResponseDto>> UpdatePostAsync(int userId, int postId, PostCreateDto postDto);
        Task<ServiceResponse<object>> DeletePostAsync(int userId, int postId);
        Task<ServiceResponse<object>> LikePostAsync(int userId, int postId);
        Task<ServiceResponse<object>> UnlikePostAsync(int userId, int postId);
        Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int postId, CommentCreateDto commentDto);
        Task<PagedList<PostResponseDto>> SearchPostsAsync(string searchTerm, PostParams postParams);
    }
}
