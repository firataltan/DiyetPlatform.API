using Microsoft.EntityFrameworkCore;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;

namespace DiyetPlatform.API.Data.Repositories
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
                .FirstOrDefaultAsync(dp => dp.Id == id) ?? throw new KeyNotFoundException($"Diet plan with ID {id} not found");
        }

        public async Task<PagedList<DietPlan>> GetDietPlansAsync(DietPlanParams dietPlanParams)
        {
            var query = _context.DietPlans
                .Include(dp => dp.User)
                    .ThenInclude(u => u.Profile)
                .Include(dp => dp.Dietitian)
                    .ThenInclude(d => d.Profile)
                .Include(dp => dp.Meals)
                .AsQueryable();

            if (dietPlanParams.IsActive.HasValue)
            {
                query = query.Where(dp => dp.IsActive == dietPlanParams.IsActive.Value);
            }

            query = dietPlanParams.OrderBy switch
            {
                "created" => query.OrderByDescending(dp => dp.CreatedAt),
                "updated" => query.OrderByDescending(dp => dp.UpdatedAt),
                "startDate" => query.OrderByDescending(dp => dp.StartDate),
                "endDate" => query.OrderByDescending(dp => dp.EndDate),
                _ => query.OrderByDescending(dp => dp.CreatedAt)
            };

            return await PagedList<DietPlan>.CreateAsync(query, dietPlanParams.PageNumber, dietPlanParams.PageSize);
        }

        public async Task<PagedList<DietPlan>> GetUserDietPlansAsync(int userId, DietPlanParams dietPlanParams)
        {
            var query = _context.DietPlans
                .Where(dp => dp.UserId == userId)
                .Include(dp => dp.User)
                    .ThenInclude(u => u.Profile)
                .Include(dp => dp.Dietitian)
                    .ThenInclude(d => d.Profile)
                .Include(dp => dp.Meals)
                .AsQueryable();

            if (dietPlanParams.IsActive.HasValue)
            {
                query = query.Where(dp => dp.IsActive == dietPlanParams.IsActive.Value);
            }

            query = dietPlanParams.OrderBy switch
            {
                "created" => query.OrderByDescending(dp => dp.CreatedAt),
                "updated" => query.OrderByDescending(dp => dp.UpdatedAt),
                "startDate" => query.OrderByDescending(dp => dp.StartDate),
                "endDate" => query.OrderByDescending(dp => dp.EndDate),
                _ => query.OrderByDescending(dp => dp.CreatedAt)
            };

            return await PagedList<DietPlan>.CreateAsync(query, dietPlanParams.PageNumber, dietPlanParams.PageSize);
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

        public async Task<PagedList<DietPlan>> SearchDietPlansAsync(string searchTerm, DietPlanParams dietPlanParams)
        {
            var query = _context.DietPlans
                .Include(dp => dp.User)
                    .ThenInclude(u => u.Profile)
                .Include(dp => dp.Dietitian)
                    .ThenInclude(d => d.Profile)
                .Include(dp => dp.Meals)
                .Where(dp => dp.Title.Contains(searchTerm) || dp.Description.Contains(searchTerm))
                .AsQueryable();

            if (dietPlanParams.IsActive.HasValue)
            {
                query = query.Where(dp => dp.IsActive == dietPlanParams.IsActive.Value);
            }

            query = dietPlanParams.OrderBy switch
            {
                "created" => query.OrderByDescending(dp => dp.CreatedAt),
                "updated" => query.OrderByDescending(dp => dp.UpdatedAt),
                "startDate" => query.OrderByDescending(dp => dp.StartDate),
                "endDate" => query.OrderByDescending(dp => dp.EndDate),
                _ => query.OrderByDescending(dp => dp.CreatedAt)
            };

            return await PagedList<DietPlan>.CreateAsync(query, dietPlanParams.PageNumber, dietPlanParams.PageSize);
        }
    }
}