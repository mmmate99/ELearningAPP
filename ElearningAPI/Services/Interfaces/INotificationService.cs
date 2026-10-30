using ElearningAPI.Hubs;

namespace ElearningAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationToUserAsync(string userId, NotificationMessage message);
        Task SendNotificationToCourseAsync(int courseId, NotificationMessage message);
        Task SendNewForumPostNotificationAsync(int courseId, string postTitle, string authorName);
        Task SendQuizResultNotificationAsync(string userId, string quizTitle, bool passed, int score);
        Task SendNewLessonNotificationAsync(int courseId, string lessonTitle);
        Task SendProgressNotificationAsync(string userId, int courseId, double completionPercentage);
    }
}