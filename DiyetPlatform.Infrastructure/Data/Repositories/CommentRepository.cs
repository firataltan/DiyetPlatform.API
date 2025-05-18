using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces.Repositories;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DiyetPlatform.Infrastructure.Data.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Comment> GetCommentByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .ThenInclude(u => u.Profile)
                .Include(c => c.Post)
                .Include(c => c.Recipe)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<Comment> GetCommentsQuery()
        {
            return _context.Comments
                .Include(c => c.User)
                .ThenInclude(u => u.Profile)
                .Include(c => c.Post)
                .Include(c => c.Recipe)
                .AsQueryable();
        }

        public Task<PagedList<Comment>> GetPostCommentsAsync(int postId, UserParams userParams)
        {
            var query = _context.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .ThenInclude(u => u.Profile)
                .OrderByDescending(c => c.CreatedAt)
                .AsQueryable();

            return Task.FromResult(PagedList<Comment>.Create(query, userParams.PageNumber, userParams.PageSize));
        }

        public async Task<bool> IsCommentExistsAsync(int id)
        {
            return await _context.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task<bool> IsUserOwnerOfCommentAsync(int userId, int commentId)
        {
            return await _context.Comments.AnyAsync(c => c.Id == commentId && c.UserId == userId);
        }

        public new void Add(Comment comment)
        {
            _context.Comments.Add(comment);
        }

        public new void Update(Comment comment)
        {
            _context.Comments.Update(comment);
        }

        public new void Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
        }
    }
} 