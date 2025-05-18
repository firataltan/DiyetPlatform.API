using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class FollowRepository : GenericRepository<Follow>, IFollowRepository
    {
        public FollowRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Follow> GetFollowAsync(int followerId, int followedId)
        {
            return await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        public IQueryable<Follow> GetFollowsQuery()
        {
            return _context.Follows.AsQueryable();
        }

        public async Task<bool> IsUserFollowingAsync(int followerId, int followedId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        public new void Remove(Follow follow)
        {
            _context.Follows.Remove(follow);
        }
    }
} 