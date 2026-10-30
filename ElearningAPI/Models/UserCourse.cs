using System;

namespace ElearningAPI.Models
{
    public class UserCourse
    {
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigációs tulajdonságok
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
    }
}