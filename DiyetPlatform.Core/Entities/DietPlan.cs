namespace DiyetPlatform.Core.Entities;

public class DietPlan
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // İlişkiler
    public int UserId { get; set; }
    public User User { get; set; }
    public int? DietitianId { get; set; }
    public User Dietitian { get; set; }
    public ICollection<DietPlanMeal> Meals { get; set; }
}
