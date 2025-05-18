using AutoMapper;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.Recipe;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DiyetPlatform.Core.Interfaces;
namespace DiyetPlatform.Application.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;

        public RecipeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment,
            IConfiguration config,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _config = config;
            _notificationService = notificationService;
        }

        public async Task<PagedList<RecipeResponseDto>> GetRecipesAsync(DiyetPlatform.Core.Common.RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.Recipes.GetRecipesAsync(recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<RecipeResponseDto> GetRecipeByIdAsync(int id)
        {
            var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(id);

            if (recipe == null)
                return null;

            return _mapper.Map<RecipeResponseDto>(recipe);
        }

        public async Task<PagedList<RecipeResponseDto>> GetUserRecipesAsync(int userId, DiyetPlatform.Core.Common.RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.Recipes.GetUserRecipesAsync(userId, recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<PagedList<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId, DiyetPlatform.Core.Common.RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.Recipes.GetRecipesByCategoryAsync(categoryId, recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<RecipeResponseDto> CreateRecipeAsync(int userId, RecipeCreateDto recipeDto)
        {
            var recipe = _mapper.Map<Recipe>(recipeDto);
            recipe.UserId = userId;
            recipe.CreatedAt = DateTime.UtcNow;

            // Görsel yükleme
            if (recipeDto.Image != null)
            {
                var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "recipe-images");

                if (!Directory.Exists(uploadsFolderPath))
                {
                    Directory.CreateDirectory(uploadsFolderPath);
                }

                var fileExtension = Path.GetExtension(recipeDto.Image.FileName);
                var fileName = $"{userId}_{DateTime.Now.Ticks}{fileExtension}";
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await recipeDto.Image.CopyToAsync(fileStream);
                }

                recipe.ImageUrl = $"/uploads/recipe-images/{fileName}";
            }

            await _unitOfWork.Recipes.AddAsync(recipe);
            await _unitOfWork.Complete();

            // Kategorileri ekle
            if (recipeDto.CategoryIds != null && recipeDto.CategoryIds.Count > 0)
            {
                foreach (var categoryId in recipeDto.CategoryIds)
                {
                    var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);

                    if (category != null)
                    {
                        var recipeCategory = new RecipeCategory
                        {
                            RecipeId = recipe.Id,
                            CategoryId = categoryId
                        };

                        await _unitOfWork.RecipeCategories.AddAsync(recipeCategory);
                    }
                }

                await _unitOfWork.Complete();
            }

            // Tarifi tüm detaylarıyla yeniden yükle
            var createdRecipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipe.Id);

            return _mapper.Map<RecipeResponseDto>(createdRecipe);
        }

        public async Task<ServiceResponse<RecipeResponseDto>> UpdateRecipeAsync(int userId, int recipeId, RecipeUpdateDto recipeDto)
        {
            var response = new ServiceResponse<RecipeResponseDto>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                if (recipe.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bu tarifi düzenleme yetkiniz yok.";
                    return response;
                }

                // Temel bilgileri güncelle
                recipe.Title = recipeDto.Title;
                recipe.Description = recipeDto.Description;
                recipe.Ingredients = JsonSerializer.Serialize(recipeDto.Ingredients);
                recipe.Instructions = string.Join("\n", recipeDto.Instructions);
                recipe.PrepTime = recipeDto.CookingTime;
                recipe.CookTime = recipeDto.CookingTime;
                recipe.NutritionInfo = JsonSerializer.Serialize(recipeDto.NutritionInfo);
                recipe.UpdatedAt = DateTime.UtcNow;

                // Görsel yükleme
                if (recipeDto.Image != null)
                {
                    var uploadsFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "recipe-images");

                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    var fileExtension = Path.GetExtension(recipeDto.Image.FileName);
                    var fileName = $"{userId}_{DateTime.Now.Ticks}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolderPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await recipeDto.Image.CopyToAsync(fileStream);
                    }

                    // Eski görseli sil (eğer varsa)
                    if (!string.IsNullOrEmpty(recipe.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, recipe.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }

                    recipe.ImageUrl = $"/uploads/recipe-images/{fileName}";
                }

                // Mevcut kategorileri sil
                var existingCategories = await _unitOfWork.RecipeCategories.Find(rc => rc.RecipeId == recipeId).ToListAsync();

                foreach (var recipeCategory in existingCategories)
                {
                    _unitOfWork.RecipeCategories.Remove(recipeCategory);
                }

                // Yeni kategorileri ekle
                if (recipeDto.CategoryIds != null && recipeDto.CategoryIds.Count > 0)
                {
                    foreach (var categoryId in recipeDto.CategoryIds)
                    {
                        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);

                        if (category != null)
                        {
                            var recipeCategory = new RecipeCategory
                            {
                                RecipeId = recipe.Id,
                                CategoryId = categoryId
                            };

                            await _unitOfWork.RecipeCategories.AddAsync(recipeCategory);
                        }
                    }
                }

                _unitOfWork.Recipes.Update(recipe);
                await _unitOfWork.Complete();

                // Tarifi tüm detaylarıyla yeniden yükle
                var updatedRecipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipe.Id);

                response.Data = _mapper.Map<RecipeResponseDto>(updatedRecipe);
                response.Message = "Tarif başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Tarif güncellenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                if (recipe.UserId != userId)
                {
                    response.Success = false;
                    response.Message = "Bu tarifi silme yetkiniz yok.";
                    return response;
                }

                // Görseli sil (eğer varsa)
                if (!string.IsNullOrEmpty(recipe.ImageUrl))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, recipe.ImageUrl.TrimStart('/'));
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                _unitOfWork.Recipes.Remove(recipe);
                await _unitOfWork.Complete();

                response.Message = "Tarif başarıyla silindi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Tarif silinirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> LikeRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                var isLiked = await _unitOfWork.Likes.Find(l => l.RecipeId == recipeId && l.UserId == userId).AnyAsync();

                if (isLiked)
                {
                    response.Success = false;
                    response.Message = "Bu tarifi zaten beğendiniz.";
                    return response;
                }

                var like = new Like
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Likes.AddAsync(like);
                await _unitOfWork.Complete();

                // Bildirim oluştur (kendi tarifini beğenmediği sürece)
                if (recipe.UserId != userId)
                {
                    await _notificationService.CreateNotificationAsync(
                        recipe.UserId,
                        userId,
                        $"{userId} tarifinizi beğendi.",
                        "like");
                }

                response.Message = "Tarif başarıyla beğenildi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Tarif beğenilirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> UnlikeRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                var like = await _unitOfWork.Likes.Find(l => l.RecipeId == recipeId && l.UserId == userId).FirstOrDefaultAsync();

                if (like == null)
                {
                    response.Success = false;
                    response.Message = "Bu tarifi zaten beğenmediniz.";
                    return response;
                }

                _unitOfWork.Likes.Remove(like);
                await _unitOfWork.Complete();

                response.Message = "Tarif beğenisi kaldırıldı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Tarif beğenisi kaldırılırken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int recipeId, CommentCreateDto commentDto)
        {
            var response = new ServiceResponse<CommentDto>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                var comment = new Comment
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    Content = commentDto.Content,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Comments.AddAsync(comment);
                await _unitOfWork.Complete();

                // Yorumu tüm detaylarıyla yeniden yükle
                var createdComment = await _unitOfWork.Comments.GetCommentByIdAsync(comment.Id);

                // Bildirim oluştur (kendi tarifine yorum yapmadığı sürece)
                if (recipe.UserId != userId)
                {
                    await _notificationService.CreateNotificationAsync(
                        recipe.UserId,
                        userId,
                        $"{userId} tarifinize yorum yaptı.",
                        "comment");
                }

                response.Data = _mapper.Map<CommentDto>(createdComment);
                response.Message = "Yorum başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Yorum eklenirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string searchTerm, DiyetPlatform.Core.Common.RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.Recipes.SearchRecipesAsync(searchTerm, recipeParams);
            return _mapper.Map<PagedList<RecipeResponseDto>>(recipes);
        }

        public async Task<ServiceResponse<object>> SaveRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                var isSaved = await _unitOfWork.SavedRecipes.Find(sr => sr.RecipeId == recipeId && sr.UserId == userId).AnyAsync();

                if (isSaved)
                {
                    response.Success = false;
                    response.Message = "Bu tarifi zaten kaydetmişsiniz.";
                    return response;
                }

                var savedRecipe = new SavedRecipe
                {
                    UserId = userId,
                    RecipeId = recipeId,
                    SavedAt = DateTime.UtcNow
                };

                await _unitOfWork.SavedRecipes.AddAsync(savedRecipe);
                await _unitOfWork.Complete();

                response.Message = "Tarif başarıyla kaydedildi.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Tarif kaydedilirken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<object>> UnsaveRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            try
            {
                var recipe = await _unitOfWork.Recipes.GetRecipeByIdAsync(recipeId);

                if (recipe == null)
                {
                    response.Success = false;
                    response.Message = "Tarif bulunamadı.";
                    return response;
                }

                var savedRecipe = await _unitOfWork.SavedRecipes.Find(sr => sr.RecipeId == recipeId && sr.UserId == userId).FirstOrDefaultAsync();

                if (savedRecipe == null)
                {
                    response.Success = false;
                    response.Message = "Bu tarifi kaydetmemişsiniz.";
                    return response;
                }

                _unitOfWork.SavedRecipes.Remove(savedRecipe);
                await _unitOfWork.Complete();

                response.Message = "Tarif kaydı kaldırıldı.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Tarif kaydı kaldırılırken bir hata oluştu: {ex.Message}";
            }

            return response;
        }

        public async Task<PagedList<RecipeResponseDto>> GetSavedRecipesAsync(int userId, DiyetPlatform.Core.Common.RecipeParams recipeParams)
        {
            var savedRecipes = await _unitOfWork.Recipes.GetSavedRecipesAsync(userId, recipeParams);
            return _mapper.Map<PagedList<RecipeResponseDto>>(savedRecipes);
        }
    }
}
