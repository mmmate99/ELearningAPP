namespace ElearningAPI.Models.DTOs
{
    public class ProgressDto
    {
        public int LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TimeSpent { get; set; }
        public int CurrentTime { get; set; }
    }

    public class UpdateProgressDto
    {
        public bool? IsCompleted { get; set; }
        public int? TimeSpent { get; set; }
        public int? CurrentTime { get; set; }
    }
}