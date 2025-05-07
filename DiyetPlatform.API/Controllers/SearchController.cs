using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.API.Helpers;
using DiyetPlatform.API.Services;

namespace DiyetPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SearchParams searchParams)
        {
            var results = await _searchService.SearchAsync(searchParams.Query, searchParams);
            return Ok(results);
        }

        [HttpGet("users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] UserParams userParams)
        {
            var users = await _searchService.SearchUsersAsync(query, userParams);
            return Ok(users);
        }

        [HttpGet("posts")]
        public async Task<IActionResult> SearchPosts([FromQuery] string query, [FromQuery] PostParams postParams)
        {
            var posts = await _searchService.SearchPostsAsync(query, postParams);
            return Ok(posts);
        }

        [HttpGet("recipes")]
        public async Task<IActionResult> SearchRecipes([FromQuery] string query, [FromQuery] RecipeParams recipeParams)
        {
            var recipes = await _searchService.SearchRecipesAsync(query, recipeParams);
            return Ok(recipes);
        }

        [HttpGet("dietplans")]
        public async Task<IActionResult> SearchDietPlans([FromQuery] string query, [FromQuery] DietPlanParams dietPlanParams)
        {
            var dietPlans = await _searchService.SearchDietPlansAsync(query, dietPlanParams);
            return Ok(dietPlans);
        }
    }
} 