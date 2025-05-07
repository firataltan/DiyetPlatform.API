using Microsoft.EntityFrameworkCore;
using DiyetPlatform.API.Models.Entities;
using DiyetPlatform.API.Helpers;

namespace DiyetPlatform.API.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<PagedList<User>> GetUsersAsync(UserParams userParams)
        {
            var query = _context.Users
                .Include(u => u.Profile)
                .AsQueryable();

            if (userParams.IsDietitian.HasValue)
            {
                query = query.Where(u => u.Profile.IsDietitian == userParams.IsDietitian.Value);
            }

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "active" => query.OrderByDescending(u => u.LastActive),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<User>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<User>> GetUserFollowersAsync(int userId, UserParams userParams)
        {
            var query = _context.Follows
                .Where(f => f.FollowedId == userId)
                .Select(f => f.Follower)
                .Include(u => u.Profile)
                .AsQueryable();

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "active" => query.OrderByDescending(u => u.LastActive),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<User>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<PagedList<User>> GetUserFollowingAsync(int userId, UserParams userParams)
        {
            var query = _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followed)
                .Include(u => u.Profile)
                .AsQueryable();

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "active" => query.OrderByDescending(u => u.LastActive),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<User>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> IsUserFollowingAsync(int followerId, int followedId)
        {
            return await _context.Follows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        public async Task<PagedList<User>> SearchUsersAsync(string searchTerm, UserParams userParams)
        {
            var query = _context.Users
                .Include(u => u.Profile)
                .Where(u => u.Username.Contains(searchTerm) ||
                            u.Profile.FullName.Contains(searchTerm) ||
                            u.Profile.Bio.Contains(searchTerm))
                .AsQueryable();

            if (userParams.IsDietitian.HasValue)
            {
                query = query.Where(u => u.Profile.IsDietitian == userParams.IsDietitian.Value);
            }

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.CreatedAt),
                "active" => query.OrderByDescending(u => u.LastActive),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PagedList<User>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
        }
    }
}