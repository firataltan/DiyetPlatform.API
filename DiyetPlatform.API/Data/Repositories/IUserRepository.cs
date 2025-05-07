using DiyetPlatform.API.Models.Entities;
using DiyetPlatform.API.Helpers;

namespace DiyetPlatform.API.Data.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<PagedList<User>> GetUsersAsync(UserParams userParams);
        Task<PagedList<User>> GetUserFollowersAsync(int userId, UserParams userParams);
        Task<PagedList<User>> GetUserFollowingAsync(int userId, UserParams userParams);
        Task<bool> IsUserFollowingAsync(int followerId, int followedId);
        Task<PagedList<User>> SearchUsersAsync(string searchTerm, UserParams userParams);
    }
}