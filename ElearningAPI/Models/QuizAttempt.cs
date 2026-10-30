using System;

namespace ElearningAPI.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public int Score { get; set; }
        public int TotalPoints { get; set; }
        public double Percentage { get; set; }
        public bool IsPassed { get; set; }
        public int AttemptNumber { get; set; } = 1;
        public string? AnswersJson { get; set; }
        
        // Navigációs tulajdonságok
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Quiz Quiz { get; set; } = null!;
    }
}