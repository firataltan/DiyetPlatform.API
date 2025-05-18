using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiyetPlatform.Core.Interfaces.Repositories
{
    public interface IDietPlanRepository : IGenericRepository<DietPlan>
    {
        Task<DietPlan> GetDietPlanByIdAsync(int id);
        IQueryable<DietPlan> GetDietPlansQuery();
        Task<PagedList<DietPlan>> GetDietPlansAsync(int pageNumber, int pageSize, string searchTerm = null, bool? isActive = null, int? dietitianId = null, string orderBy = null);
        Task<PagedList<DietPlan>> GetUserDietPlansAsync(int userId, int pageNumber, int pageSize, string searchTerm = null, bool? isActive = null, string orderBy = null);
        Task<PagedList<DietPlan>> SearchDietPlansAsync(string searchTerm, int pageNumber, int pageSize, bool? isActive = null, int? dietitianId = null, string orderBy = null);
        Task<bool> IsDietPlanExistsAsync(int id);
        Task<bool> IsUserOwnerOfDietPlanAsync(int userId, int dietPlanId);
        Task<bool> IsUserDietitianOfDietPlanAsync(int userId, int dietPlanId);
        DbSet<DietPlanMeal> DietPlanMeals { get; }
        Task AddDietPlanMealAsync(DietPlanMeal meal);
        Task<List<DietPlanMeal>> GetDietPlanMealsAsync(int dietPlanId);
        void RemoveDietPlanMeal(DietPlanMeal meal);
        new void Add(DietPlan dietPlan);
        new void Update(DietPlan dietPlan);
        new void Delete(DietPlan dietPlan);
    }
}