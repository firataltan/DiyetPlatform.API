using Microsoft.EntityFrameworkCore;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
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
                .Include(u => u.Following)
                .Include(u => u.Followers)
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

        public IQueryable<User> GetUsersQuery()
        {
            return _context.Users
                .Include(u => u.Profile)
                .AsQueryable();
        }

        public Task<PagedList<User>> GetFollowersAsync(int userId, UserParams userParams)
        {
            var query = _context.Follows
                .Where(f => f.FollowedId == userId)
                .Select(f => f.Follower)
                .Include(u => u.Profile)
                .AsQueryable();

            return Task.FromResult(PagedList<User>.Create(query, userParams.PageNumber, userParams.PageSize));
        }

        public Task<PagedList<User>> GetFollowingAsync(int userId, UserParams userParams)
        {
            var query = _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followed)
                .Include(u => u.Profile)
                .AsQueryable();

            return Task.FromResult(PagedList<User>.Create(query, userParams.PageNumber, userParams.PageSize));
        }

        public Task<PagedList<User>> SearchUsersAsync(string searchTerm, UserParams userParams)
        {
            var query = _context.Users
                .Include(u => u.Profile)
                .Where(u => u.Username.Contains(searchTerm) ||
                             u.Profile.FullName.Contains(searchTerm))
                .AsQueryable();

            return Task.FromResult(PagedList<User>.Create(query, userParams.PageNumber, userParams.PageSize));
        }

        public new void Delete(User user)
        {
            _context.Users.Remove(user);
        }
    }
}