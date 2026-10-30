using System;

namespace ElearningAPI.Models
{
    public class Progress
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpent { get; set; }
        public int CurrentTime { get; set; }
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
        
        // Navigációs tulajdonságok
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Lesson Lesson { get; set; } = null!;
    }
}