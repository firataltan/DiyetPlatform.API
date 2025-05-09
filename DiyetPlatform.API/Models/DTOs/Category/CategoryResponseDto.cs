namespace DiyetPlatform.API.Models.DTOs.Category
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RecipesCount { get; set; }
    }
} 