using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models.DTOs
{
    public class ForumPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsPinned { get; set; }
        public UserDto Author { get; set; } = null!;
        public int CourseId { get; set; }
        public int? ParentPostId { get; set; }
        public List<ForumPostDto> Replies { get; set; } = new List<ForumPostDto>();
    }

    public class CreateForumPostDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }
        
        public int? ParentPostId { get; set; }
    }

    public class UpdateForumPostDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }
    }
}