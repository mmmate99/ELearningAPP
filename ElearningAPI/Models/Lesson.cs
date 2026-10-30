using System.Collections.Generic;

namespace ElearningAPI.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int Order { get; set; }
        public LessonType Type { get; set; }
        public string? VideoUrl { get; set; }
        public string? AttachmentUrl { get; set; }
        public int EstimatedDuration { get; set; }
        
        // Foreign keys
        public int ModuleId { get; set; }
        
        // Navigációs tulajdonságok
        public virtual Module Module { get; set; } = null!;
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
    }

    public enum LessonType
    {
        Text,
        Video,
        Document,
        Interactive
    }
}