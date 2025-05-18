using System;

namespace DiyetPlatform.Application.DTOs.Post
{
    public class PostResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string UserProfileImage { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLiked { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

     
}