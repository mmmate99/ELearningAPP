using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models.DTOs
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public int CourseId { get; set; }
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    }

    public class CreateModuleDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public int CourseId { get; set; }
    }

    public class UpdateModuleDto
    {
        [StringLength(200)]
        public string? Title { get; set; }
        
        public string? Description { get; set; }
        
        public int? Order { get; set; }
    }
}