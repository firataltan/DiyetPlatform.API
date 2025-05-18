using System;

namespace DiyetPlatform.Core.Entities;

public class Profile
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Bio { get; set; }
    public string ProfileImageUrl { get; set; }
    public bool IsDietitian { get; set; }
    public string DietitianCertificateUrl { get; set; }
    public bool IsCertificateVerified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // İlişkiler
    public int UserId { get; set; }
    public User User { get; set; }
}
