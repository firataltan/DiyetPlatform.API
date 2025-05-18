using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.DTOs.Category;
using DiyetPlatform.Application.Common.Parameters;

namespace DiyetPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryParams categoryParams = null)
        {
            categoryParams ??= new CategoryParams();
            var categories = await _categoryService.GetCategoriesAsync(categoryParams);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto categoryDto)
        {
            var result = await _categoryService.CreateCategoryAsync(categoryDto);
            if (!result.Success)
                return BadRequest(result.Message);

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data.Id }, result.Data);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
} 