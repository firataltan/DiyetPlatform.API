using Microsoft.EntityFrameworkCore;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;

namespace DiyetPlatform.API.Data.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                        .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedList<Post>> GetPostsAsync(PostParams postParams)
        {
            var query = _context.Posts
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .AsQueryable();

            query = postParams.OrderBy switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await PagedList<Post>.CreateAsync(query, postParams.PageNumber, postParams.PageSize);
        }

        public async Task<PagedList<Post>> GetUserPostsAsync(int userId, PostParams postParams)
        {
            var query = _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .AsQueryable();

            query = postParams.OrderBy switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await PagedList<Post>.CreateAsync(query, postParams.PageNumber, postParams.PageSize);
        }

        public async Task<PagedList<Post>> GetUserFeedAsync(int userId, PostParams postParams)
        {
            // Kullanıcının kendi gönderileri ve takip ettiği kullanıcıların gönderileri
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            followingIds.Add(userId); // Kendi gönderilerini de ekle

            var query = _context.Posts
                .Where(p => followingIds.Contains(p.UserId))
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .AsQueryable();

            query = postParams.OrderBy switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await PagedList<Post>.CreateAsync(query, postParams.PageNumber, postParams.PageSize);
        }

        public async Task<bool> IsPostExistsAsync(int id)
        {
            return await _context.Posts.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> IsUserOwnerOfPostAsync(int userId, int postId)
        {
            return await _context.Posts.AnyAsync(p => p.Id == postId && p.UserId == userId);
        }

        public async Task<bool> IsPostLikedByUserAsync(int userId, int postId)
        {
            return await _context.Likes.AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task<Like> GetPostLikeAsync(int userId, int postId)
        {
            return await _context.Likes
                .SingleOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);
        }

        public async Task<PagedList<Post>> SearchPostsAsync(string searchTerm, PostParams postParams)
        {
            var query = _context.Posts
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.Content.Contains(searchTerm))
                .AsQueryable();

            query = postParams.OrderBy switch
            {
                "created" => query.OrderByDescending(p => p.CreatedAt),
                "updated" => query.OrderByDescending(p => p.UpdatedAt),
                "likes" => query.OrderByDescending(p => p.Likes.Count),
                "comments" => query.OrderByDescending(p => p.Comments.Count),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            return await PagedList<Post>.CreateAsync(query, postParams.PageNumber, postParams.PageSize);
        }

        public DbSet<Comment> Comments => _context.Comments;

        public async Task AddLikeAsync(Like like)
        {
            await _context.Likes.AddAsync(like);
        }
        
        public async Task AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public void DeleteLike(Like like)
        {
            _context.Likes.Remove(like);
        }
    }
}