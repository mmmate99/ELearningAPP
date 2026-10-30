using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Services.Interfaces
{
    public interface IProgressService
    {
        Task<LessonDto?> GetLessonWithProgressAsync(int lessonId, string userId);
        Task<ProgressDto> UpdateLessonProgressAsync(int lessonId, string userId, UpdateProgressDto progressDto);
        Task<bool> MarkLessonAsCompletedAsync(int lessonId, string userId);
        Task<double> GetCourseCompletionPercentageAsync(int courseId, string userId);
        Task<IEnumerable<ProgressDto>> GetCourseProgressAsync(int courseId, string userId);
        Task<IEnumerable<LessonDto>> GetRecentLessonsAsync(string userId, int count = 5);
    }
}