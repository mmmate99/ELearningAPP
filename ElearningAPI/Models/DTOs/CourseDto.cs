using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public double? AverageRating { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEnrolled { get; set; }
        public decimal? UserRating { get; set; }
    }

    public class CourseDetailsDto : CourseDto
    {
        public List<ModuleDto> Modules { get; set; } = new List<ModuleDto>();
        public List<QuizDto> Quizzes { get; set; } = new List<QuizDto>();
        public List<ForumPostDto> ForumPosts { get; set; } = new List<ForumPostDto>();
    }

    public class CreateCourseDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        [Range(0, 10000)]
        public decimal Price { get; set; } = 0;
        
        public bool IsFree { get; set; }
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Beginner;
    }

    public class UpdateCourseDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        
        [Range(0, 10000)]
        public decimal? Price { get; set; }
        
        public bool? IsFree { get; set; }
        public DifficultyLevel? Difficulty { get; set; }
        public bool? IsPublished { get; set; }
    }

    public class CourseEnrollmentDto
    {
        public int CourseId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class CourseRatingDto
    {
        [Range(1, 5)]
        public decimal Rating { get; set; }
        
        [StringLength(1000)]
        public string? Review { get; set; }
    }
}