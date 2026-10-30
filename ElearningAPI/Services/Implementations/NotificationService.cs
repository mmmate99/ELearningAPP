using Microsoft.AspNetCore.SignalR;
using ElearningAPI.Hubs;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendNotificationToUserAsync(string userId, NotificationMessage message)
        {
            try
            {
                await NotificationHub.SendNotificationToUserAsync(_hubContext, userId, message.Message, message.Type);
                _logger.LogInformation($"Notification sent to user {userId}: {message.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
            }
        }

        public async Task SendNotificationToCourseAsync(int courseId, NotificationMessage message)
        {
            try
            {
                await NotificationHub.SendCourseNotificationAsync(_hubContext, courseId, message.Message, message.Data);
                _logger.LogInformation($"Notification sent to course {courseId}: {message.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to course {courseId}");
            }
        }

        public async Task SendNewForumPostNotificationAsync(int courseId, string postTitle, string authorName)
        {
            var message = new NotificationMessage
            {
                Type = "info",
                Title = "Új fórum poszt",
                Message = $"{authorName} új posztot írt: {postTitle}",
                Data = new { CourseId = courseId, PostTitle = postTitle }
            };

            await SendNotificationToCourseAsync(courseId, message);
        }

        public async Task SendQuizResultNotificationAsync(string userId, string quizTitle, bool passed, int score)
        {
            var result = passed ? "sikeres" : "sikertelen";
            var message = new NotificationMessage
            {
                Type = passed ? "success" : "warning",
                Title = "Kvíz eredmény",
                Message = $"A(z) {quizTitle} kvíz kitöltése {result} volt. Pontszám: {score}",
                Data = new { QuizTitle = quizTitle, Passed = passed, Score = score }
            };

            await SendNotificationToUserAsync(userId, message);
        }

        public async Task SendNewLessonNotificationAsync(int courseId, string lessonTitle)
        {
            var message = new NotificationMessage
            {
                Type = "info",
                Title = "Új lecke",
                Message = $"Új lecke érhető el: {lessonTitle}",
                Data = new { CourseId = courseId, LessonTitle = lessonTitle }
            };

            await SendNotificationToCourseAsync(courseId, message);
        }

        public async Task SendProgressNotificationAsync(string userId, int courseId, double completionPercentage)
        {
            var message = new NotificationMessage
            {
                Type = "success",
                Title = "Haladás",
                Message = $"A kurzus {completionPercentage:0}%-át teljesítetted!",
                Data = new { CourseId = courseId, CompletionPercentage = completionPercentage }
            };

            await SendNotificationToUserAsync(userId, message);
        }
    }
}