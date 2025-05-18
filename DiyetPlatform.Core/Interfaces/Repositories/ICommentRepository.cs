using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using System.Threading.Tasks;
using System.Linq;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<Comment> GetCommentByIdAsync(int id);
        IQueryable<Comment> GetCommentsQuery();
        Task<PagedList<Comment>> GetPostCommentsAsync(int postId, UserParams userParams);
        Task<bool> IsCommentExistsAsync(int id);
        Task<bool> IsUserOwnerOfCommentAsync(int userId, int commentId);
        new void Add(Comment comment);
        new void Update(Comment comment);
        new void Delete(Comment comment);
    }
} 