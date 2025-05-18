using System.ComponentModel.DataAnnotations;

namespace DiyetPlatform.Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 