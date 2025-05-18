using AutoMapper;
using DiyetPlatform.Core.Entities;
using DiyetPlatform.Core.Interfaces;
using DiyetPlatform.Application.Common.Models;
using DiyetPlatform.Application.Common.Parameters;
using DiyetPlatform.Application.DTOs.DietPlan;
using DiyetPlatform.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DiyetPlatform.Application.Services
{
    public class DietPlanService : IDietPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public DietPlanService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<PagedList<DietPlanResponseDto>> GetDietPlansAsync(DietPlanParams dietPlanParams)
        {
            var dietPlans = await _unitOfWork.DietPlans.GetDietPlansAsync(
                dietPlanParams.PageNumber,
                dietPlanParams.PageSize,
                dietPlanParams.Search,
                dietPlanParams.IsActive,
                dietPlanParams.DietitianId,
                dietPlanParams.OrderBy);
                
            var dietPlanDtos = _mapper.Map<PagedList<DietPlanResponseDto>>(dietPlans);

            return dietPlanDtos;
        }

        public async Task<DietPlanResponseDto> GetDietPlanByIdAsync(int id)
        {
            var dietPlan = await _unitOfWork.DietPlans.GetDietPlanByIdAsync(id);

            if (dietPlan == null)
                return null;

            return _mapper.Map<DietPlanResponseDto>(dietPlan);
        }

        public async Task<PagedList<DietPlanResponseDto>> GetUserDietPlansAsync(int userId, DietPlanParams dietPlanParams)
        {
            var dietPlans = await _unitOfWork.DietPlans.GetUserDietPlansAsync(
                userId,
                dietPlanParams.PageNumber,
                dietPlanParams.PageSize,
                dietPlanParams.Search,
                dietPlanParams.IsActive,
                dietPlanParams.OrderBy);
                
            var dietPlanDtos = _mapper.Map<PagedList<DietPlanResponseDto>>(dietPlans);

            return dietPlanDtos;
        }

        public async Task<DietPlanResponseDto> CreateDietPlanAsync(int userId, DietPlanCreateDto dietPlanDto)
        {
            var dietPlan = _mapper.Map<DietPlan>(dietPlanDto);
            dietPlan.UserId = userId;
            dietPlan.CreatedAt = DateTime.UtcNow;

            // Kullanıcı diyetisyen mi kontrol et
            var user = await _unitOfWork.Users.GetUserByIdAsync(userId);

            if (user.Profile.IsDietitian)
            {
                dietPlan.DietitianId = userId;
            }

            await _unitOfWork.DietPlans.AddAsync(dietPlan);
            await _unitOfWork.Complete();

            // Öğünleri ekle
            if (dietPlanDto.Meals != null && dietPlanDto.Meals.Count > 0)
            {
                foreach (var mealDto in dietPlanDto.Meals)
                {
                    var meal = new DietPlanMeal
                    {
                        DietPlanId = dietPlan.Id,
                        Title = mealDto.Title,
                        //MealTime = mealDto.MealTime,
                        FoodItems = JsonSerializer.Serialize(mealDto.FoodItems),
                        NutritionInfo = JsonSerializer.Serialize(mealDto.NutritionInfo)
                    };

                    await _unitOfWork.DietPlans.AddDietPlanMealAsync(meal);
                }
                
                await _unitOfWork.Complete();
            }

            // Diyet planını tüm detaylarıyla yeniden yükle
            var createdDietPlan = await _unitOfWork.DietPlans.GetDietPlanByIdAsync(dietPlan.Id);

            return _mapper.Map<DietPlanResponseDto>(createdDietPlan);
        }

        public async Task<ServiceResponse<DietPlanResponseDto>> UpdateDietPlanAsync(int userId, int dietPlanId, DietPlanCreateDto dietPlanDto)
        {
            var response = new ServiceResponse<DietPlanResponseDto>();

            var dietPlan = await _unitOfWork.DietPlans.GetDietPlanByIdAsync(dietPlanId);

            if (dietPlan == null)
            {
                response.Success = false;
                response.Message = "Diyet planı bulunamadı.";
                return response;
            }

            var isOwner = await _unitOfWork.DietPlans.IsUserOwnerOfDietPlanAsync(userId, dietPlanId);
            var isDietitian = await _unitOfWork.DietPlans.IsUserDietitianOfDietPlanAsync(userId, dietPlanId);

            if (!isOwner && !isDietitian)
            {
                response.Success = false;
                response.Message = "Bu diyet planını düzenleme yetkiniz yok.";
                return response;
            }

            // Temel bilgileri güncelle
            dietPlan.Title = dietPlanDto.Title;
            dietPlan.Description = dietPlanDto.Description;
            dietPlan.StartDate = dietPlanDto.StartDate;
            dietPlan.EndDate = dietPlanDto.EndDate;
            dietPlan.IsActive = dietPlanDto.IsActive;
            dietPlan.UpdatedAt = DateTime.UtcNow;

            // Mevcut öğünleri sil
            var existingMeals = await _unitOfWork.DietPlans.GetDietPlanMealsAsync(dietPlanId);

            foreach (var meal in existingMeals)
            {
                _unitOfWork.DietPlans.RemoveDietPlanMeal(meal);
            }

            // Yeni öğünleri ekle
            if (dietPlanDto.Meals != null)
            {
                foreach (var mealDto in dietPlanDto.Meals)
                {
                    var meal = new DietPlanMeal
                    {
                        DietPlanId = dietPlanId,
                        Title = mealDto.Title,
                        //MealTime = mealDto.MealTime,
                        FoodItems = JsonSerializer.Serialize(mealDto.FoodItems),
                        NutritionInfo = JsonSerializer.Serialize(mealDto.NutritionInfo)
                    };

                    await _unitOfWork.DietPlans.AddDietPlanMealAsync(meal);
                }
            }

            await _unitOfWork.Complete();

            // Diyet planını tüm detaylarıyla yeniden yükle
            var updatedDietPlan = await _unitOfWork.DietPlans.GetDietPlanByIdAsync(dietPlan.Id);

            response.Data = _mapper.Map<DietPlanResponseDto>(updatedDietPlan);
            response.Message = "Diyet planı başarıyla güncellendi.";

            return response;
        }

        public async Task<ServiceResponse<object>> DeleteDietPlanAsync(int userId, int dietPlanId)
        {
            var response = new ServiceResponse<object>();

            var dietPlan = await _unitOfWork.DietPlans.GetDietPlanByIdAsync(dietPlanId);

            if (dietPlan == null)
            {
                response.Success = false;
                response.Message = "Diyet planı bulunamadı.";
                return response;
            }

            // Kullanıcı, planın sahibi veya diyetisyeni mi kontrol et
            if (dietPlan.UserId != userId && dietPlan.DietitianId != userId)
            {
                response.Success = false;
                response.Message = "Bu diyet planını silme yetkiniz yok.";
                return response;
            }

            _unitOfWork.DietPlans.Delete(dietPlan);
            await _unitOfWork.Complete();

            response.Message = "Diyet planı başarıyla silindi.";

            return response;
        }

        public async Task<ServiceResponse<DietPlanResponseDto>> CreateDietPlanForUserAsync(int dietitianId, int userId, DietPlanCreateDto dietPlanDto)
        {
            var response = new ServiceResponse<DietPlanResponseDto>();

            // Oluşturan kullanıcı diyetisyen mi kontrol et
            var dietitian = await _unitOfWork.Users.GetUserByIdAsync(dietitianId);

            if (!dietitian.Profile.IsDietitian)
            {
                response.Success = false;
                response.Message = "Diyet planı oluşturmak için diyetisyen olmalısınız.";
                return response;
            }

            // Hedef kullanıcı var mı kontrol et
            var targetUser = await _unitOfWork.Users.GetUserByIdAsync(userId);
            if (targetUser == null)
            {
                response.Success = false;
                response.Message = "Hedef kullanıcı bulunamadı.";
                return response;
            }

            var dietPlan = _mapper.Map<DietPlan>(dietPlanDto);
            dietPlan.UserId = userId;
            dietPlan.DietitianId = dietitianId;
            dietPlan.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.DietPlans.AddAsync(dietPlan);

            // Öğünleri ekle
            if (dietPlanDto.Meals != null && dietPlanDto.Meals.Count > 0)
            {
                foreach (var mealDto in dietPlanDto.Meals)
                {
                    var meal = new DietPlanMeal
                    {
                        DietPlanId = dietPlan.Id,
                        Title = mealDto.Title,
                        //MealTime = mealDto.MealTime,
                        FoodItems = JsonSerializer.Serialize(mealDto.FoodItems),
                        NutritionInfo = JsonSerializer.Serialize(mealDto.NutritionInfo)
                    };

                    await _unitOfWork.DietPlans.AddDietPlanMealAsync(meal);
                }
            }

            await _unitOfWork.Complete();

            // Bildirim oluştur
            await _notificationService.CreateNotificationAsync(
                userId, // Bildirim alıcısı
                dietitianId, // Bildirim göndereni
                $"{dietitian.Username} size yeni bir diyet planı oluşturdu: {dietPlanDto.Title}", // Bildirim içeriği
                "dietplan_created", // Bildirim tipi
                null, // postId - null olarak geçiyoruz
                null, // recipeId - null olarak geçiyoruz
                null  // commentId - null olarak geçiyoruz
            );

            // Diyet planını tüm detaylarıyla yeniden yükle
            var createdDietPlan = await _unitOfWork.DietPlans.GetDietPlanByIdAsync(dietPlan.Id);

            response.Data = _mapper.Map<DietPlanResponseDto>(createdDietPlan);
            response.Message = "Diyet planı başarıyla oluşturuldu.";

            return response;
        }

        public async Task<PagedList<DietPlanResponseDto>> SearchDietPlansAsync(string searchTerm, DietPlanParams dietPlanParams)
        {
            var dietPlans = await _unitOfWork.DietPlans.SearchDietPlansAsync(
                searchTerm, 
                dietPlanParams.PageNumber, 
                dietPlanParams.PageSize, 
                dietPlanParams.IsActive, 
                dietPlanParams.DietitianId, 
                dietPlanParams.OrderBy);
                
            var dietPlanDtos = _mapper.Map<PagedList<DietPlanResponseDto>>(dietPlans);

            return dietPlanDtos;
        }
    }
}