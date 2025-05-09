using AutoMapper;
using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Models.DTOs.Category;
using DiyetPlatform.API.Models.Entities;
using DiyetPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DiyetPlatform.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CategoryResponseDto>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.RecipeRepository.RecipeCategories
                .Include(rc => rc.Category)
                .Include(rc => rc.Recipe)
                .GroupBy(rc => rc.Category)
                .Select(g => new CategoryResponseDto
                {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Description = g.Key.Description,
                    RecipesCount = g.Count()
                })
                .ToListAsync();

            return categories;
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.RecipeRepository.GetCategoryByIdAsync(id);
            if (category == null)
                return null;

            var recipesCount = await _unitOfWork.RecipeRepository.RecipeCategories
                .CountAsync(rc => rc.CategoryId == id);

            var categoryDto = _mapper.Map<CategoryResponseDto>(category);
            categoryDto.RecipesCount = recipesCount;

            return categoryDto;
        }

        public async Task<ServiceResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var response = new ServiceResponse<CategoryResponseDto>();

            var category = _mapper.Map<Category>(categoryDto);

            await _unitOfWork.RecipeRepository.AddCategoryAsync(category);
            await _unitOfWork.Complete();

            response.Data = _mapper.Map<CategoryResponseDto>(category);
            response.Message = "Kategori başarıyla oluşturuldu.";

            return response;
        }

        public async Task<ServiceResponse<CategoryResponseDto>> UpdateCategoryAsync(int id, CategoryUpdateDto categoryDto)
        {
            var response = new ServiceResponse<CategoryResponseDto>();

            var category = await _unitOfWork.RecipeRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Kategori bulunamadı.";
                return response;
            }

            _mapper.Map(categoryDto, category);
            _unitOfWork.RecipeRepository.UpdateCategory(category);
            await _unitOfWork.Complete();

            response.Data = _mapper.Map<CategoryResponseDto>(category);
            response.Message = "Kategori başarıyla güncellendi.";

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteCategoryAsync(int id)
        {
            var response = new ServiceResponse<object>();

            var category = await _unitOfWork.RecipeRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                response.Success = false;
                response.Message = "Kategori bulunamadı.";
                return response;
            }

            // Kategoriye ait recipe'leri kontrol et
            var hasRecipes = await _unitOfWork.RecipeRepository.RecipeCategories
                .AnyAsync(rc => rc.CategoryId == id);

            if (hasRecipes)
            {
                response.Success = false;
                response.Message = "Bu kategoriye ait tarifler olduğu için silinemez.";
                return response;
            }

            _unitOfWork.RecipeRepository.DeleteCategory(category);
            await _unitOfWork.Complete();

            response.Message = "Kategori başarıyla silindi.";

            return response;
        }
    }
} 