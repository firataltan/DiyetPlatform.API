using Microsoft.AspNetCore.Http;

namespace DiyetPlatform.Application.DTOs.Post
{
    public class PostCreateDto
    {
        public string Content { get; set; }
        public IFormFile Image { get; set; }
    }
}