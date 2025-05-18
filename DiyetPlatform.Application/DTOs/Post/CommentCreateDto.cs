using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.Application.DTOs.Post
{
    public class CommentCreateDto
    {
        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Content { get; set; }
    }
} 