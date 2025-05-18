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
    public class DietPlanRepository : GenericRepository<DietPlan>, IDietPlanRepository
    {
        public DietPlanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public DbSet<DietPlanMeal> DietPlanMeals => _context.DietPlanMeals;

        public async Task AddDietPlanMealAsync(DietPlanMeal meal)
        {
            await _context.DietPlanMeals.AddAsync(meal);
        }

        public async Task<DietPlan> GetDietPlanByIdAsync(int id)
        {
            return await _context.DietPlans
                .Include(dp => dp.User)
                    .ThenInclude(u => u.Profile)
                .Include(dp => dp.Dietitian)
                    .ThenInclude(d => d.Profile)
                .Include(dp => dp.Meals)
                .FirstOrDefaultAsync(dp => dp.Id == id);
        }

        public IQueryable<DietPlan> GetDietPlansQuery()
        {
            return _context.DietPlans
                .Include(dp => dp.User)
                    .ThenInclude(u => u.Profile)
                .Include(dp => dp.Dietitian)
                    .ThenInclude(d => d.Profile)
                .Include(dp => dp.Meals)
                .AsQueryable();
        }

        public async Task<PagedList<DietPlan>> GetDietPlansAsync(int pageNumber, int pageSize, string searchTerm = null, bool? isActive = null, int? dietitianId = null, string orderBy = null)
        {
            var query = GetDietPlansQuery();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(dp => dp.Title.ToLower().Contains(searchTerm.ToLower()) || 
                                         dp.Description.ToLower().Contains(searchTerm.ToLower()));

            if (isActive.HasValue)
                query = query.Where(dp => dp.IsActive == isActive.Value);

            if (dietitianId.HasValue)
                query = query.Where(dp => dp.DietitianId == dietitianId.Value);

            // Apply sorting
            query = orderBy switch
            {
                "titleDesc" => query.OrderByDescending(dp => dp.Title),
                "date" => query.OrderBy(dp => dp.CreatedAt),
                "dateDesc" => query.OrderByDescending(dp => dp.CreatedAt),
                _ => query.OrderBy(dp => dp.Title)
            };

            return await PagedList<DietPlan>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedList<DietPlan>> GetUserDietPlansAsync(int userId, int pageNumber, int pageSize, string searchTerm = null, bool? isActive = null, string orderBy = null)
        {
            var query = _context.DietPlans
                .Where(dp => dp.UserId == userId)
                .Include(dp => dp.User)
                    .ThenInclude(u => u.Profile)
                .Include(dp => dp.Dietitian)
                    .ThenInclude(d => d.Profile)
                .Include(dp => dp.Meals)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(dp => dp.Title.ToLower().Contains(searchTerm.ToLower()) || 
                                         dp.Description.ToLower().Contains(searchTerm.ToLower()));

            if (isActive.HasValue)
                query = query.Where(dp => dp.IsActive == isActive.Value);

            // Apply sorting
            query = orderBy switch
            {
                "titleDesc" => query.OrderByDescending(dp => dp.Title),
                "date" => query.OrderBy(dp => dp.CreatedAt),
                "dateDesc" => query.OrderByDescending(dp => dp.CreatedAt),
                _ => query.OrderBy(dp => dp.Title)
            };

            return await PagedList<DietPlan>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<PagedList<DietPlan>> SearchDietPlansAsync(string searchTerm, int pageNumber, int pageSize, bool? isActive = null, int? dietitianId = null, string orderBy = null)
        {
            var query = GetDietPlansQuery()
                .Where(dp => dp.Title.ToLower().Contains(searchTerm.ToLower()) || 
                           dp.Description.ToLower().Contains(searchTerm.ToLower()));

            // Apply filters
            if (isActive.HasValue)
                query = query.Where(dp => dp.IsActive == isActive.Value);

            if (dietitianId.HasValue)
                query = query.Where(dp => dp.DietitianId == dietitianId.Value);

            // Apply sorting
            query = orderBy switch
            {
                "titleDesc" => query.OrderByDescending(dp => dp.Title),
                "date" => query.OrderBy(dp => dp.CreatedAt),
                "dateDesc" => query.OrderByDescending(dp => dp.CreatedAt),
                _ => query.OrderBy(dp => dp.Title)
            };

            return await PagedList<DietPlan>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<List<DietPlanMeal>> GetDietPlanMealsAsync(int dietPlanId)
        {
            return await _context.DietPlanMeals
                .Where(m => m.DietPlanId == dietPlanId)
                .ToListAsync();
        }

        public void RemoveDietPlanMeal(DietPlanMeal meal)
        {
            _context.DietPlanMeals.Remove(meal);
        }

        public async Task<bool> IsDietPlanExistsAsync(int id)
        {
            return await _context.DietPlans.AnyAsync(dp => dp.Id == id);
        }

        public async Task<bool> IsUserOwnerOfDietPlanAsync(int userId, int dietPlanId)
        {
            return await _context.DietPlans.AnyAsync(dp => dp.Id == dietPlanId && dp.UserId == userId);
        }

        public async Task<bool> IsUserDietitianOfDietPlanAsync(int userId, int dietPlanId)
        {
            return await _context.DietPlans.AnyAsync(dp => dp.Id == dietPlanId && dp.DietitianId == userId);
        }

        public new void Add(DietPlan dietPlan)
        {
            _context.DietPlans.Add(dietPlan);
        }

        public new void Update(DietPlan dietPlan)
        {
            _context.DietPlans.Update(dietPlan);
        }

        public new void Delete(DietPlan dietPlan)
        {
            _context.DietPlans.Remove(dietPlan);
        }
    }
}