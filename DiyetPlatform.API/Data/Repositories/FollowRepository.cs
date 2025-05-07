using DiyetPlatform.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DiyetPlatform.API.Data.Repositories
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
    }
} 