using DiyetPlatform.API.Models.DTOs.Recipe;
using DiyetPlatform.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.API.Helpers;

namespace DiyetPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecipes([FromQuery] RecipeParams recipeParams)
        {
            var recipes = await _recipeService.GetRecipesAsync(recipeParams);
            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipe(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);

            if (recipe == null)
                return NotFound("Tarif bulunamadı.");

            return Ok(recipe);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRecipe([FromForm] RecipeCreateDto recipeDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var recipe = await _recipeService.CreateRecipeAsync(userId, recipeDto);

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromForm] RecipeUpdateDto recipeDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var result = await _recipeService.UpdateRecipeAsync(userId, id, recipeDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value);
            var result = await _recipeService.DeleteRecipeAsync(userId, id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRecipes([FromQuery] string query, [FromQuery] RecipeParams recipeParams)
        {
            var recipes = await _recipeService.SearchRecipesAsync(query, recipeParams);
            return Ok(recipes);
        }

        [HttpGet("categories/{categoryId}")]
        public async Task<IActionResult> GetRecipesByCategory(int categoryId, [FromQuery] RecipeParams recipeParams)
        {
            var recipes = await _recipeService.GetRecipesByCategoryAsync(categoryId, recipeParams);
            return Ok(recipes);
        }
    }
}