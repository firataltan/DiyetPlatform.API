using AutoMapper;
using DiyetPlatform.Core.Common;
using DiyetPlatform.Core.Interfaces;
using DiyetPlatform.Application.DTOs.User;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.DTOs.Recipe;
using DiyetPlatform.Application.DTOs.DietPlan;
using DiyetPlatform.Application.DTOs.Search;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;

namespace DiyetPlatform.Application.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SearchService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SearchResultDto> SearchAllAsync(string searchTerm)
        {
            var searchParams = new SearchParams
            {
                PageNumber = 1,
                PageSize = 10,
                IncludeUsers = true,
                IncludePosts = true,
                IncludeRecipes = true,
                IncludeDietPlans = true
            };

            return await SearchAsync(searchTerm, searchParams);
        }

        public async Task<SearchResultDto> SearchAsync(string query, SearchParams searchParams)
        {
            var result = new SearchResultDto();

            // Kullanıcıları ara
            if (searchParams.IncludeUsers)
            {
                var userParams = new DiyetPlatform.Core.Common.UserParams
                {
                    PageNumber = searchParams.PageNumber,
                    PageSize = searchParams.PageSize
                };

                var users = await _unitOfWork.Users.SearchUsersAsync(query, userParams);
                result.Users = _mapper.Map<PagedList<UserDto>>(users);
            }

            // Gönderileri ara
            if (searchParams.IncludePosts)
            {
                var postParams = new DiyetPlatform.Core.Common.PostParams
                {
                    PageNumber = searchParams.PageNumber,
                    PageSize = searchParams.PageSize
                };

                var posts = await _unitOfWork.Posts.SearchPostsAsync(query, postParams);
                result.Posts = _mapper.Map<PagedList<PostResponseDto>>(posts);
            }

            // Tarifleri ara
            if (searchParams.IncludeRecipes)
            {
                var recipeParams = new DiyetPlatform.Core.Common.RecipeParams
                {
                    PageNumber = searchParams.PageNumber,
                    PageSize = searchParams.PageSize
                };

                var recipes = await _unitOfWork.Recipes.SearchRecipesAsync(query, recipeParams);
                result.Recipes = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);
            }

            // Diyet planlarını ara
            if (searchParams.IncludeDietPlans)
            {
                var dietPlanParams = new DietPlanParams
                {
                    PageNumber = searchParams.PageNumber,
                    PageSize = searchParams.PageSize
                };

                var dietPlans = await _unitOfWork.DietPlans.SearchDietPlansAsync(
                    query, 
                    dietPlanParams.PageNumber, 
                    dietPlanParams.PageSize, 
                    dietPlanParams.IsActive, 
                    dietPlanParams.DietitianId, 
                    dietPlanParams.OrderBy);
                    
                result.DietPlans = _mapper.Map<PagedList<DietPlanResponseDto>>(dietPlans);
            }

            return result;
        }

        public async Task<PagedList<UserDto>> SearchUsersAsync(string query, DiyetPlatform.Core.Common.UserParams userParams)
        {
            var users = await _unitOfWork.Users.SearchUsersAsync(query, userParams);
            var userDtos = _mapper.Map<PagedList<UserDto>>(users);

            return userDtos;
        }

        public async Task<PagedList<PostResponseDto>> SearchPostsAsync(string query, DiyetPlatform.Core.Common.PostParams postParams)
        {
            var posts = await _unitOfWork.Posts.SearchPostsAsync(query, postParams);
            var postDtos = _mapper.Map<PagedList<PostResponseDto>>(posts);

            return postDtos;
        }

        public async Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string query, DiyetPlatform.Core.Common.RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.Recipes.SearchRecipesAsync(query, recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<PagedList<DietPlanResponseDto>> SearchDietPlansAsync(string query, DietPlanParams dietPlanParams)
        {
            var dietPlans = await _unitOfWork.DietPlans.SearchDietPlansAsync(
                query, 
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