using Microsoft.AspNetCore.Http;

namespace DiyetPlatform.API.Models.DTOs.User
{
    public class ProfileUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public string Specialization { get; set; }
        public string Certification { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}