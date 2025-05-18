using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DiyetPlatform.Application.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string FullName { get; set; }

        public IFormFile ProfileImage { get; set; }
        public bool IsDietitian { get; set; }
        public IFormFile DietitianCertificate { get; set; }
    }
}