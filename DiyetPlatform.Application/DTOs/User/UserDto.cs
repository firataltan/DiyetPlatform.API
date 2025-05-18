using System;

namespace DiyetPlatform.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsDietitian { get; set; }
        public bool IsVerified { get; set; }
        public string DietitianCertificateUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActive { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int RecipesCount { get; set; }
        public int PostsCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}