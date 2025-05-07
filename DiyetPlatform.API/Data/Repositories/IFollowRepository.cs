using DiyetPlatform.API.Models.Entities;
using System.Threading.Tasks;

namespace DiyetPlatform.API.Data.Repositories
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<Follow> GetFollowAsync(int followerId, int followedId);
    }
} 