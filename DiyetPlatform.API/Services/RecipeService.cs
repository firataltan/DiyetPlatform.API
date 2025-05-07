using AutoMapper;
using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using DiyetPlatform.API.Models;

namespace DiyetPlatform.API.Services
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

        public async Task<PagedList<RecipeResponseDto>> GetRecipesAsync(RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.RecipeRepository.GetRecipesAsync(recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<RecipeResponseDto> GetRecipeByIdAsync(int id)
        {
            var recipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(id);

            if (recipe == null)
                return null;

            return _mapper.Map<RecipeResponseDto>(recipe);
        }

        public async Task<PagedList<RecipeResponseDto>> GetUserRecipesAsync(int userId, RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.RecipeRepository.GetUserRecipesAsync(userId, recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<PagedList<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId, RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.RecipeRepository.GetRecipesByCategoryAsync(categoryId, recipeParams);
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

            await _unitOfWork.RecipeRepository.AddAsync(recipe);
            await _unitOfWork.Complete();

            // Kategorileri ekle
            if (recipeDto.CategoryIds != null && recipeDto.CategoryIds.Count > 0)
            {
                foreach (var categoryId in recipeDto.CategoryIds)
                {
                    var category = await _unitOfWork.RecipeRepository.GetCategoryByIdAsync(categoryId);

                    if (category != null)
                    {
                        var recipeCategory = new RecipeCategory
                        {
                            RecipeId = recipe.Id,
                            CategoryId = categoryId
                        };

                        await _unitOfWork.RecipeRepository.AddRecipeCategoryAsync(recipeCategory);
                    }
                }

                await _unitOfWork.Complete();
            }

            // Tarifi tüm detaylarıyla yeniden yükle
            var createdRecipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipe.Id);

            return _mapper.Map<RecipeResponseDto>(createdRecipe);
        }

        public async Task<ServiceResponse<RecipeResponseDto>> UpdateRecipeAsync(int userId, int recipeId, RecipeUpdateDto recipeDto)
        {
            var response = new ServiceResponse<RecipeResponseDto>();

            var recipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipeId);

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
            recipe.Instructions = recipeDto.Instructions;
            recipe.PrepTimeMinutes = recipeDto.PrepTimeMinutes;
            recipe.CookTimeMinutes = recipeDto.CookTimeMinutes;
            recipe.Calories = recipeDto.Calories;
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
            var existingCategories = await _unitOfWork.RecipeRepository.RecipeCategories.Where(rc => rc.RecipeId == recipeId).ToListAsync();

            foreach (var recipeCategory in existingCategories)
            {
                _unitOfWork.RecipeRepository.DeleteRecipeCategory(recipeCategory);
            }

            // Yeni kategorileri ekle
            if (recipeDto.CategoryIds != null && recipeDto.CategoryIds.Count > 0)
            {
                foreach (var categoryId in recipeDto.CategoryIds)
                {
                    var category = await _unitOfWork.RecipeRepository.GetCategoryByIdAsync(categoryId);

                    if (category != null)
                    {
                        var recipeCategory = new RecipeCategory
                        {
                            RecipeId = recipe.Id,
                            CategoryId = categoryId
                        };

                        await _unitOfWork.RecipeRepository.AddRecipeCategoryAsync(recipeCategory);
                    }
                }
            }

            await _unitOfWork.Complete();

            // Tarifi tüm detaylarıyla yeniden yükle
            var updatedRecipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipe.Id);

            response.Data = _mapper.Map<RecipeResponseDto>(updatedRecipe);
            response.Message = "Tarif başarıyla güncellendi.";

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            var recipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipeId);

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

            _unitOfWork.RecipeRepository.Delete(recipe);
            await _unitOfWork.Complete();

            response.Message = "Tarif başarıyla silindi.";

            return response;
        }

        public async Task<ServiceResponse<object>> LikeRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            var recipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipeId);

            if (recipe == null)
            {
                response.Success = false;
                response.Message = "Tarif bulunamadı.";
                return response;
            }

            var isLiked = await _unitOfWork.RecipeRepository.IsRecipeLikedByUserAsync(userId, recipeId);

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

            await _unitOfWork.RecipeRepository.AddLikeAsync(like);
            await _unitOfWork.Complete();

            // Bildirim oluştur (kendi tarifini beğenmediği sürece)
            if (recipe.UserId != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    recipe.UserId, // Bildirim alıcısı
                    userId, // Bildirim göndereni
                    "Like",
                    null, recipeId, null,
                    "tarifinizi beğendi."
                );
            }

            response.Message = "Tarif başarıyla beğenildi.";

            return response;
        }

        public async Task<ServiceResponse<object>> UnlikeRecipeAsync(int userId, int recipeId)
        {
            var response = new ServiceResponse<object>();

            var recipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipeId);

            if (recipe == null)
            {
                response.Success = false;
                response.Message = "Tarif bulunamadı.";
                return response;
            }

            var like = await _unitOfWork.RecipeRepository.GetRecipeLikeAsync(userId, recipeId);

            if (like == null)
            {
                response.Success = false;
                response.Message = "Bu tarifi zaten beğenmediniz.";
                return response;
            }

            _unitOfWork.RecipeRepository.DeleteLike(like);
            await _unitOfWork.Complete();

            response.Message = "Tarif beğenisi kaldırıldı.";

            return response;
        }

        public async Task<ServiceResponse<CommentDto>> AddCommentAsync(int userId, int recipeId, CommentCreateDto commentDto)
        {
            var response = new ServiceResponse<CommentDto>();

            var recipe = await _unitOfWork.RecipeRepository.GetRecipeByIdAsync(recipeId);

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

            await _unitOfWork.RecipeRepository.AddCommentAsync(comment);
            await _unitOfWork.Complete();

            // Yorumun sahibi ve kullanıcı bilgilerini yükle
            comment = await _unitOfWork.RecipeRepository.Comments
                .Include(c => c.User)
                    .ThenInclude(u => u.Profile)
                .FirstOrDefaultAsync(c => c.Id == comment.Id);

            // Bildirim oluştur (kendi tarifine yorum yapmadığı sürece)
            if (recipe.UserId != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    recipe.UserId, // Bildirim alıcısı
                    userId, // Bildirim göndereni
                    "Comment",
                    null, recipeId, comment.Id,
                    "tarifinize yorum yaptı."
                );
            }

            response.Data = _mapper.Map<CommentDto>(comment);
            response.Message = "Yorum başarıyla eklendi.";

            return response;
        }

        public async Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string searchTerm, RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.RecipeRepository.SearchRecipesAsync(searchTerm, recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }
    }
}
