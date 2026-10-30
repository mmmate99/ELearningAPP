using System;
using System.Collections.Generic;

namespace ElearningAPI.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PassPercentage { get; set; } = 70;
        public int TimeLimit { get; set; }
        public bool IsPublished { get; set; }
        public int MaxAttempts { get; set; } = 3;
        
        // Foreign keys
        public int? LessonId { get; set; }
        public int? CourseId { get; set; }
        
        // Navigációs tulajdonságok
        public virtual Lesson? Lesson { get; set; }
        public virtual Course? Course { get; set; }
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }
}