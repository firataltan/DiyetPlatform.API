using DiyetPlatform.Application.DTOs.User;

namespace DiyetPlatform.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; }
    }
} 