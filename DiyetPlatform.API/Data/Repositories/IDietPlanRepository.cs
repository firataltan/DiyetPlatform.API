using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiyetPlatform.API.Data.Repositories
{
    public interface IDietPlanRepository : IGenericRepository<DietPlan>
    {
        Task<DietPlan> GetDietPlanByIdAsync(int id);
        Task<PagedList<DietPlan>> GetDietPlansAsync(DietPlanParams dietPlanParams);
        Task<PagedList<DietPlan>> GetUserDietPlansAsync(int userId, DietPlanParams dietPlanParams);
        Task<bool> IsDietPlanExistsAsync(int id);
        Task<bool> IsUserOwnerOfDietPlanAsync(int userId, int dietPlanId);
        Task<bool> IsUserDietitianOfDietPlanAsync(int userId, int dietPlanId);
        Task<PagedList<DietPlan>> SearchDietPlansAsync(string searchTerm, DietPlanParams dietPlanParams);
        DbSet<DietPlanMeal> DietPlanMeals { get; }
        Task AddDietPlanMealAsync(DietPlanMeal meal);
    }
}
