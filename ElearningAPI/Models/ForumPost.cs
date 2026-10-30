using System;
using System.Collections.Generic;

namespace ElearningAPI.Models
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsPinned { get; set; }
        
        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int? ParentPostId { get; set; }
        
        // Navigációs tulajdonságok
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
        public virtual ForumPost? ParentPost { get; set; }
        public virtual ICollection<ForumPost> Replies { get; set; } = new List<ForumPost>();
    }
}