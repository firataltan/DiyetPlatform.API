using DiyetPlatform.Application.Common.Models;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.DietPlan;

namespace DiyetPlatform.Application.Interfaces
{
    public interface IDietPlanService
    {
        Task<PagedList<DietPlanResponseDto>> GetDietPlansAsync(DietPlanParams dietPlanParams);
        Task<DietPlanResponseDto> GetDietPlanByIdAsync(int id);
        Task<PagedList<DietPlanResponseDto>> GetUserDietPlansAsync(int userId, DietPlanParams dietPlanParams);
        Task<DietPlanResponseDto> CreateDietPlanAsync(int userId, DietPlanCreateDto dietPlanDto);
        Task<ServiceResponse<DietPlanResponseDto>> UpdateDietPlanAsync(int userId, int dietPlanId, DietPlanCreateDto dietPlanDto);
        Task<ServiceResponse<object>> DeleteDietPlanAsync(int userId, int dietPlanId);
        Task<ServiceResponse<DietPlanResponseDto>> CreateDietPlanForUserAsync(int dietitianId, int userId, DietPlanCreateDto dietPlanDto);
        Task<PagedList<DietPlanResponseDto>> SearchDietPlansAsync(string searchTerm, DietPlanParams dietPlanParams);
    }
}
