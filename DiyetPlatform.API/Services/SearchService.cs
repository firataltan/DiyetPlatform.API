using AutoMapper;
using DiyetPlatform.API.Data.UnitOfWork;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Models.DTOs;
using DiyetPlatform.API.Models.DTOs.User;
using DiyetPlatform.API.Models.DTOs.Post;
using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Models.DTOs.DietPlan;

namespace DiyetPlatform.API.Services
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

        public async Task<SearchResultDto> SearchAsync(string query, Helpers.SearchParams searchParams)
        {
            var result = new SearchResultDto();

            // Kullanıcıları ara
            var userParams = new UserParams
            {
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize
            };
            result.Users = await SearchUsersAsync(query, userParams);

            // Gönderileri ara
            var postParams = new PostParams
            {
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize
            };
            result.Posts = await SearchPostsAsync(query, postParams);

            // Tarifleri ara
            var recipeParams = new RecipeParams
            {
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize
            };
            result.Recipes = await SearchRecipesAsync(query, recipeParams);

            // Diyet planlarını ara
            var dietPlanParams = new DietPlanParams
            {
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize
            };
            result.DietPlans = await SearchDietPlansAsync(query, dietPlanParams);

            return result;
        }

        public async Task<PagedList<UserDto>> SearchUsersAsync(string query, UserParams userParams)
        {
            var users = await _unitOfWork.UserRepository.SearchUsersAsync(query, userParams);
            var userDtos = _mapper.Map<PagedList<UserDto>>(users);

            return userDtos;
        }

        public async Task<PagedList<PostResponseDto>> SearchPostsAsync(string query, PostParams postParams)
        {
            var posts = await _unitOfWork.PostRepository.SearchPostsAsync(query, postParams);
            var postDtos = _mapper.Map<PagedList<PostResponseDto>>(posts);

            return postDtos;
        }

        public async Task<PagedList<RecipeResponseDto>> SearchRecipesAsync(string query, RecipeParams recipeParams)
        {
            var recipes = await _unitOfWork.RecipeRepository.SearchRecipesAsync(query, recipeParams);
            var recipeDtos = _mapper.Map<PagedList<RecipeResponseDto>>(recipes);

            return recipeDtos;
        }

        public async Task<PagedList<DietPlanResponseDto>> SearchDietPlansAsync(string query, DietPlanParams dietPlanParams)
        {
            var dietPlans = await _unitOfWork.DietPlanRepository.SearchDietPlansAsync(query, dietPlanParams);
            var dietPlanDtos = _mapper.Map<PagedList<DietPlanResponseDto>>(dietPlans);

            return dietPlanDtos;
        }
    }
}