using System;

namespace DiyetPlatform.Core.Entities
{
    public class Follow
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // liþkiler
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
        public User Follower { get; set; }
        public User Followed { get; set; }
    }
}
