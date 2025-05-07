namespace DiyetPlatform.API.Models.DTOs.Post
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public IFormFile Media { get; set; }
    }
}