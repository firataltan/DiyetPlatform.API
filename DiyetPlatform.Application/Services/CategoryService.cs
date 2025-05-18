using AutoMapper;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.Category;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DiyetPlatform.Application.Services
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

        public async Task<PagedList<CategoryResponseDto>> GetCategoriesAsync(CategoryParams categoryParams)
        {
            var query = _unitOfWork.Categories.GetCategoriesQuery();

            // Apply filters
            if (!string.IsNullOrEmpty(categoryParams.Search))
                query = query.Where(c => c.Name.ToLower().Contains(categoryParams.Search));

            if (categoryParams.IsActive.HasValue)
                query = query.Where(c => c.IsActive == categoryParams.IsActive.Value);

            // Apply sorting
            query = categoryParams.OrderBy switch
            {
                "nameDesc" => query.OrderByDescending(c => c.Name),
                "date" => query.OrderBy(c => c.CreatedAt),
                "dateDesc" => query.OrderByDescending(c => c.CreatedAt),
                "recipesCount" => query.OrderBy(c => c.Recipes.Count),
                "recipesCountDesc" => query.OrderByDescending(c => c.Recipes.Count),
                _ => query.OrderBy(c => c.Name)
            };

            var totalCount = await query.CountAsync();
            var categories = await query
                .Skip((categoryParams.PageNumber - 1) * categoryParams.PageSize)
                .Take(categoryParams.PageSize)
                .ToListAsync();

            var categoryDtos = _mapper.Map<List<CategoryResponseDto>>(categories);

            // Add recipe counts
            for (int i = 0; i < categories.Count; i++)
            {
                categoryDtos[i].RecipesCount = categories[i].Recipes.Count;
            }

            return new PagedList<CategoryResponseDto>(
                categoryDtos,
                totalCount,
                categoryParams.PageNumber,
                categoryParams.PageSize);
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return null;

            var categoryDto = _mapper.Map<CategoryResponseDto>(category);
            categoryDto.RecipesCount = category.Recipes.Count;

            return categoryDto;
        }

        public async Task<ServiceResponse<CategoryResponseDto>> CreateCategoryAsync(CategoryCreateDto categoryDto)
        {
            var response = new ServiceResponse<CategoryResponseDto>();

            try
            {
                var category = _mapper.Map<Category>(categoryDto);
                category.CreatedAt = DateTime.UtcNow;

                // Handle image upload if provided
                if (categoryDto.Image != null)
                {
                    // Image upload logic will be handled in a separate service
                    category.ImageUrl = await _unitOfWork.Categories.UploadImageAsync(categoryDto.Image);
                }

                _unitOfWork.Categories.Add(category);
                await _unitOfWork.Complete();

                response.Data = _mapper.Map<CategoryResponseDto>(category);
                response.Message = "Kategori başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kategori oluşturulurken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CategoryResponseDto>> UpdateCategoryAsync(int categoryId, CategoryUpdateDto categoryDto)
        {
            var response = new ServiceResponse<CategoryResponseDto>();

            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Kategori bulunamadı.";
                    return response;
                }

                _mapper.Map(categoryDto, category);
                category.UpdatedAt = DateTime.UtcNow;

                // Handle image upload if provided
                if (categoryDto.Image != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(category.ImageUrl))
                    {
                        await _unitOfWork.Categories.DeleteImageAsync(category.ImageUrl);
                    }

                    // Upload new image
                    category.ImageUrl = await _unitOfWork.Categories.UploadImageAsync(categoryDto.Image);
                }

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.Complete();

                response.Data = _mapper.Map<CategoryResponseDto>(category);
                response.Message = "Kategori başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kategori güncellenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteCategoryAsync(int categoryId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
                if (category == null)
                {
                    response.Success = false;
                    response.Message = "Kategori bulunamadı.";
                    return response;
                }

                // Check if category has recipes
                if (category.Recipes.Count > 0)
                {
                    response.Success = false;
                    response.Message = "Bu kategoriye ait tarifler olduğu için silinemez.";
                    return response;
                }

                // Delete image if exists
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    await _unitOfWork.Categories.DeleteImageAsync(category.ImageUrl);
                }

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.Complete();

                response.Message = "Kategori başarıyla silindi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Kategori silinirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }
    }
} 