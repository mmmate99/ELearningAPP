using System.Collections.Generic;

namespace ElearningAPI.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        
        // Foreign keys
        public int CourseId { get; set; }
        
        // Navigációs tulajdonságok
        public virtual Course Course { get; set; } = null!;
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}