using Microsoft.AspNetCore.Http;

namespace DiyetPlatform.Application.DTOs.User
{
    public class ProfileUpdateDto
    {
        public string FullName { get; set; }
        public string Bio { get; set; }
        public IFormFile ProfileImage { get; set; }
        public bool IsDietitian { get; set; }
        public IFormFile DietitianCertificate { get; set; }
    }
}