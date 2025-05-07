using System;

namespace DiyetPlatform.API.Models.Entities
{
    public class Profile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Bio { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsDietitian { get; set; }
        public string Specialization { get; set; }
        public string Certification { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public User User { get; set; }
        
        // Hesaplanan özellik
        public string FullName => $"{FirstName} {LastName}";
    }
}
