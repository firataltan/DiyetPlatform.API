using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post> GetPostByIdAsync(int id);
        IQueryable<Post> GetPostsQuery();
        Task<PagedList<Post>> GetUserPostsAsync(int userId, UserParams userParams);
        Task<PagedList<Post>> GetFeedPostsAsync(int userId, UserParams userParams);
        Task<PagedList<Post>> SearchPostsAsync(string searchTerm, DiyetPlatform.Core.Common.PostParams postParams);
        Task<string> UploadImageAsync(IFormFile image);
        Task DeleteImageAsync(string imageUrl);
        Task<bool> IsPostLikedByUserAsync(int userId, int postId);
        Task<Like> GetPostLikeAsync(int userId, int postId);
        Task AddLikeAsync(Like like);
        void DeleteLike(Like like);
        Task AddCommentAsync(Comment comment);
        new void Add(Post post);
        new void Update(Post post);
        new void Delete(Post post);
    }
} 