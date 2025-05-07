namespace DiyetPlatform.API.Models.Entities
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // İlişkiler
        public User? Follower { get; set; }
        public User? Followed { get; set; }
    }
}

