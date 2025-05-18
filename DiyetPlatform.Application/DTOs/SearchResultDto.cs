using DiyetPlatform.Core.Common;
using DiyetPlatform.Application.DTOs.User;
using DiyetPlatform.Application.DTOs.Post;
using DiyetPlatform.Application.DTOs.Recipe;
using DiyetPlatform.Application.DTOs.DietPlan;

namespace DiyetPlatform.Application.DTOs;

public class SearchResultDto
{
    public PagedList<UserDto> Users { get; set; }
    public PagedList<PostResponseDto> Posts { get; set; }
    public PagedList<RecipeResponseDto> Recipes { get; set; }
    public PagedList<DietPlanResponseDto> DietPlans { get; set; }
    public int TotalResults => (Users?.TotalCount ?? 0) + (Posts?.TotalCount ?? 0) + (Recipes?.TotalCount ?? 0) + (DietPlans?.TotalCount ?? 0);
}