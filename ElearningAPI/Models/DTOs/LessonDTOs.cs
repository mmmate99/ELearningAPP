using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models.DTOs
{
    public class LessonDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public int Order { get; set; }
        public LessonType Type { get; set; }
        public string? VideoUrl { get; set; }
        public string? AttachmentUrl { get; set; }
        public int EstimatedDuration { get; set; }
        public int ModuleId { get; set; }
        public bool IsCompleted { get; set; }
        public int? TimeSpent { get; set; }
        public int? CurrentTime { get; set; }
        public int? PreviousLessonId { get; set; }
        public int? NextLessonId { get; set; }
        public string? ModuleTitle { get; set; }
    }

    public class CreateLessonDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Content { get; set; }
        
        [Required]
        public int ModuleId { get; set; }
        
        public int Order { get; set; }
        public LessonType Type { get; set; } = LessonType.Text;
        public string? VideoUrl { get; set; }
        public string? AttachmentUrl { get; set; }
        
        [Range(1, 480)]
        public int EstimatedDuration { get; set; } = 30;
    }

    /*public class UpdateProgressDto
    {
        public bool? IsCompleted { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? TimeSpent { get; set; }
        
        [Range(0, int.MaxValue)]
        public int? CurrentTime { get; set; }
    }*/

    public class UpdateLessonDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }
        
        public int? Order { get; set; }
        
        public LessonType? Type { get; set; }
        
        public string? VideoUrl { get; set; }
        
        public string? AttachmentUrl { get; set; }
        
        [Range(1, 480)]
        public int? EstimatedDuration { get; set; }
    }
}