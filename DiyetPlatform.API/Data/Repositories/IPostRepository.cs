using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiyetPlatform.API.Data.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post> GetPostByIdAsync(int id);
        Task<PagedList<Post>> GetPostsAsync(PostParams postParams);
        Task<PagedList<Post>> GetUserPostsAsync(int userId, PostParams postParams);
        Task<PagedList<Post>> GetUserFeedAsync(int userId, PostParams postParams);
        Task<bool> IsPostExistsAsync(int id);
        Task<bool> IsUserOwnerOfPostAsync(int userId, int postId);
        Task<bool> IsPostLikedByUserAsync(int userId, int postId);
        Task<Like> GetPostLikeAsync(int userId, int postId);
        Task<PagedList<Post>> SearchPostsAsync(string searchTerm, PostParams postParams);
        DbSet<Comment> Comments { get; }
        Task AddLikeAsync(Like like);
        void DeleteLike(Like like);
        Task AddCommentAsync(Comment comment);
    }
}