using Microsoft.EntityFrameworkCore;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly IHostEnvironment _hostEnvironment;

        public PostRepository(ApplicationDbContext context, IHostEnvironment hostEnvironment) : base(context)
        {
            _hostEnvironment = hostEnvironment;
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

        public IQueryable<Post> GetPostsQuery()
        {
            return _context.Posts
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .AsQueryable();
        }

        public Task<PagedList<Post>> GetUserPostsAsync(int userId, UserParams userParams)
        {
            var query = _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            return Task.FromResult(PagedList<Post>.Create(query, userParams.PageNumber, userParams.PageSize));
        }

        public async Task<PagedList<Post>> GetFeedPostsAsync(int userId, UserParams userParams)
        {
            // Get ids of users the current user follows
            var followingIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            // Add current user to see their own posts
            followingIds.Add(userId);

            var query = _context.Posts
                .Where(p => followingIds.Contains(p.UserId))
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            return PagedList<Post>.Create(query, userParams.PageNumber, userParams.PageSize);
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

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;

            var uploadsFolderPath = Path.Combine(_hostEnvironment.ContentRootPath, "uploads", "post-images");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var fileExtension = Path.GetExtension(image.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/uploads/post-images/{fileName}";
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return;

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, imageUrl.TrimStart('/'));

            if (File.Exists(imagePath))
            {
                await Task.Run(() => File.Delete(imagePath));
            }
        }

        public Task<PagedList<Post>> SearchPostsAsync(string searchTerm, DiyetPlatform.Core.Common.PostParams postParams)
        {
            var query = _context.Posts
                .Include(p => p.User)
                    .ThenInclude(u => u.Profile)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .Where(p => p.Content.Contains(searchTerm))
                .AsQueryable();

            return Task.FromResult(PagedList<Post>.Create(query, postParams.PageNumber, postParams.PageSize));
        }

        public new void Delete(Post post)
        {
            _context.Posts.Remove(post);
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