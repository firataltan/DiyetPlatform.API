using DiyetPlatform.API.Models.DTOs.Category;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryResponseDto>> GetCategoriesAsync();
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<ServiceResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<ServiceResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, CategoryUpdateDto categoryDto);
        Task<ServiceResponse<object>> DeleteCategoryAsync(int id);
    }
} 