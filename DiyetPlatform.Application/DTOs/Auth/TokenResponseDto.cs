using System;

namespace DiyetPlatform.API.Models.DTOs.Auth
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public int UserId { get; set; }
        public DateTime Expiration { get; set; }
    }
}
