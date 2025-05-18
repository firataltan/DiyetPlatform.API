using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class LikeRepository : GenericRepository<Like>, ILikeRepository
    {
        public LikeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Like> GetLikeAsync(int userId, int postId)
        {
            return await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public IQueryable<Like> GetLikesQuery()
        {
            return _context.Likes.AsQueryable();
        }

        public async Task<bool> IsLikeExistsAsync(int userId, int postId)
        {
            return await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public async Task<bool> HasUserLikedPostAsync(int userId, int postId)
        {
            return await _context.Likes.AnyAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public new void Delete(Like like)
        {
            _context.Likes.Remove(like);
        }
    }
} 