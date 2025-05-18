using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
