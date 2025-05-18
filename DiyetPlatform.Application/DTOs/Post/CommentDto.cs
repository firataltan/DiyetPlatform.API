using System;

namespace DiyetPlatform.Application.DTOs.Post
{
    public class CommentDto
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? UserProfileImage { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 