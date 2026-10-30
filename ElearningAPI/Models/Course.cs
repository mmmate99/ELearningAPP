using System;
using System.Collections.Generic;

namespace ElearningAPI.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; }
        
        // Foreign keys
        public string TeacherId { get; set; } = string.Empty;
        
        // Navigációs tulajdonságok
        public virtual ApplicationUser Teacher { get; set; } = null!;
        public virtual ICollection<Module> Modules { get; set; } = new List<Module>();
        public virtual ICollection<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
        public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }

    public enum DifficultyLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }
}