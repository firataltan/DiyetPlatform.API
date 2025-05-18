using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DiyetPlatform.Application.DTOs.DietPlan;
using DiyetPlatform.Application.Interfaces;
using DiyetPlatform.Application.Common.Parameters;

namespace DiyetPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietPlansController : ControllerBase
    {
        private readonly IDietPlanService _dietPlanService;

        public DietPlansController(IDietPlanService dietPlanService)
        {
            _dietPlanService = dietPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDietPlans([FromQuery] DietPlanParams dietPlanParams)
        {
            var dietPlans = await _dietPlanService.GetDietPlansAsync(dietPlanParams);
            return Ok(dietPlans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDietPlan(int id)
        {
            var dietPlan = await _dietPlanService.GetDietPlanByIdAsync(id);

            if (dietPlan == null)
                return NotFound("Diyet planı bulunamadı.");

            return Ok(dietPlan);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDietPlans(int userId, [FromQuery] DietPlanParams dietPlanParams)
        {
            var dietPlans = await _dietPlanService.GetUserDietPlansAsync(userId, dietPlanParams);
            return Ok(dietPlans);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDietPlan([FromBody] DietPlanCreateDto dietPlanDto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var dietPlan = await _dietPlanService.CreateDietPlanAsync(userId, dietPlanDto);

            return CreatedAtAction(nameof(GetDietPlan), new { id = dietPlan.Id }, dietPlan);
        }

        [Authorize]
        [HttpPost("user/{userId}")]
        public async Task<IActionResult> CreateDietPlanForUser(int userId, [FromBody] DietPlanCreateDto dietPlanDto)
        {
            var dietitianId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            
            // Diyetisyen rolü kontrolü
            if (!User.IsInRole("Dietitian"))
                return Forbid("Bu işlemi gerçekleştirmek için diyetisyen olmalısınız.");
                
            var result = await _dietPlanService.CreateDietPlanForUserAsync(dietitianId, userId, dietPlanDto);
            
            if (!result.Success)
                return BadRequest(result.Message);
                
            return CreatedAtAction(nameof(GetDietPlan), new { id = result.Data.Id }, result.Data);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDietPlan(int id, [FromBody] DietPlanCreateDto dietPlanDto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _dietPlanService.UpdateDietPlanAsync(userId, id, dietPlanDto);
            
            if (!result.Success)
                return BadRequest(result.Message);
                
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDietPlan(int id)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _dietPlanService.DeleteDietPlanAsync(userId, id);
            
            if (!result.Success)
                return BadRequest(result.Message);
                
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDietPlans([FromQuery] string query, [FromQuery] DietPlanParams dietPlanParams)
        {
            var dietPlans = await _dietPlanService.SearchDietPlansAsync(query, dietPlanParams);
            return Ok(dietPlans);
        }
    }
}