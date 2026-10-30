using ElearningAPI.Models;

namespace ElearningAPI.Repository.Interfaces
{
    public interface IProgressRepository
    {
        Task<Progress?> GetUserProgressAsync(string userId, int lessonId);
        Task<IEnumerable<Progress>> GetUserCourseProgressAsync(string userId, int courseId);
        Task<Progress> CreateOrUpdateProgressAsync(Progress progress);
        Task<bool> MarkLessonCompletedAsync(string userId, int lessonId);
        Task<double> GetCourseCompletionPercentageAsync(string userId, int courseId);
        Task<IEnumerable<Progress>> GetRecentProgressAsync(string userId, int count = 10);
    }
}