using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        IQueryable<User> GetUsersQuery();
        Task<PagedList<User>> GetFollowersAsync(int userId, UserParams userParams);
        Task<PagedList<User>> GetFollowingAsync(int userId, UserParams userParams);
        Task<PagedList<User>> SearchUsersAsync(string searchTerm, UserParams userParams);
    }
} 