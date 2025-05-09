using AutoMapper;
using DiyetPlatform.API.Models.DTOs;
using DiyetPlatform.API.Models.DTOs.Auth;
using DiyetPlatform.API.Models.DTOs.Category;
using DiyetPlatform.API.Models.DTOs.DietPlan;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Models.DTOs.User;
using DiyetPlatform.API.Models.Entities;
using System.Text.Json;
using Profile = AutoMapper.Profile;

namespace DiyetPlatform.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Profile.FullName))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Profile.Bio))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.Profile.ProfileImageUrl))
                .ForMember(dest => dest.IsDietitian, opt => opt.MapFrom(src => src.Profile.IsDietitian))
                .ForMember(dest => dest.PostsCount, opt => opt.MapFrom(src => src.Posts.Count))
                .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers.Count))
                .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.Following.Count))
                .ForMember(dest => dest.IsFollowing, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        context.Items.ContainsKey("currentUserId") && 
                        context.Items["currentUserId"] != null));

            CreateMap<ProfileUpdateDto, Profile>();

            // Post mappings
            CreateMap<Post, PostResponseDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.Profile.ProfileImageUrl))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.IsLiked, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        context.Items.ContainsKey("currentUserId") && 
                        src.Likes.Any(l => l.UserId == (int)context.Items["currentUserId"])));

            CreateMap<PostCreateDto, Post>();

            // Comment mappings
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.Profile.ProfileImageUrl));

            // DietPlan mappings
            CreateMap<DietPlanCreateDto, DietPlan>();
            
            // DietPlanMeal mappings
            CreateMap<DietPlanMealDto, DietPlanMeal>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.MealTime, opt => opt.MapFrom(src => src.MealTime))
                .ForMember(dest => dest.FoodItems, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        JsonSerializer.Serialize(src.FoodItems)))
                .ForMember(dest => dest.NutritionInfo, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        JsonSerializer.Serialize(src.NutritionInfo)));

            CreateMap<DietPlan, DietPlanResponseDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.DietitianName, opt => opt.MapFrom(src => src.Dietitian != null ? src.Dietitian.Profile.FullName : null));

            CreateMap<DietPlanMeal, DietPlanMealDto>()
                .ForMember(dest => dest.FoodItems, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        !string.IsNullOrEmpty(src.FoodItems) ? 
                        JsonSerializer.Deserialize<List<string>>(src.FoodItems) : 
                        new List<string>()))
                .ForMember(dest => dest.NutritionInfo, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        !string.IsNullOrEmpty(src.NutritionInfo) ? 
                        JsonSerializer.Deserialize<Dictionary<string, string>>(src.NutritionInfo) : 
                        new Dictionary<string, string>()));

            // Recipe mappings
            CreateMap<RecipeCreateDto, Recipe>()
                .ForMember(dest => dest.Ingredients, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        JsonSerializer.Serialize(src.Ingredients)))
                .ForMember(dest => dest.NutritionInfo, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        JsonSerializer.Serialize(src.NutritionInfo)));

            CreateMap<Recipe, RecipeResponseDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.UserProfileImage, opt => opt.MapFrom(src => src.User.Profile.ProfileImageUrl))
                .ForMember(dest => dest.Ingredients, opt => 
                    opt.MapFrom((src, dest, destMember, context) =>
                        !string.IsNullOrEmpty(src.Ingredients) ?
                        JsonSerializer.Deserialize<List<string>>(src.Ingredients) :
                        new List<string>()))
                .ForMember(dest => dest.NutritionInfo, opt => 
                    opt.MapFrom((src, dest, destMember, context) =>
                        !string.IsNullOrEmpty(src.NutritionInfo) ?
                        JsonSerializer.Deserialize<Dictionary<string, string>>(src.NutritionInfo) :
                        new Dictionary<string, string>()))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.Categories.Select(rc => rc.Category.Name).ToList()))
                .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
                .ForMember(dest => dest.IsLiked, opt => 
                    opt.MapFrom((src, dest, destMember, context) => 
                        context.Items.ContainsKey("currentUserId") && 
                        src.Likes.Any(l => l.UserId == (int)context.Items["currentUserId"])));

            // Category mappings
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<Category, CategoryResponseDto>();
        }
    }
}
