using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.Category;

namespace DiyetPlatform.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedList<CategoryResponseDto>> GetCategoriesAsync(CategoryParams categoryParams);
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<ServiceResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto categoryDto);
        Task<ServiceResponse<CategoryResponseDto>> UpdateCategoryAsync(int categoryId, CategoryUpdateDto categoryDto);
        Task<ServiceResponse<object>> DeleteCategoryAsync(int categoryId);
    }
} 