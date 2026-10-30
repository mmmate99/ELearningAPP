using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ElearningAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserRole Role { get; set; } = UserRole.Student;
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        
        // Navigációs tulajdonságok
        public virtual ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
        public virtual ICollection<UserCourse> EnrolledCourses { get; set; } = new List<UserCourse>();
        public virtual ICollection<ForumPost> ForumPosts { get; set; } = new List<ForumPost>();
        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }

    public enum UserRole
    {
        Student,
        Teacher,
        Admin
    }
}