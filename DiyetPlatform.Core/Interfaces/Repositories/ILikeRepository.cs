using DiyetPlatform.Core.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface ILikeRepository : IGenericRepository<Like>
    {
        Task<Like> GetLikeAsync(int userId, int postId);
        IQueryable<Like> GetLikesQuery();
        Task<bool> HasUserLikedPostAsync(int userId, int postId);
        new void Add(Like like);
        new void Delete(Like like);
    }
} 