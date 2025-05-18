using DiyetPlatform.Core.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<Follow> GetFollowAsync(int followerId, int followedId);
        IQueryable<Follow> GetFollowsQuery();
        Task<bool> IsUserFollowingAsync(int followerId, int followedId);
        new void Add(Follow follow);
        new void Remove(Follow follow);
    }
} 