namespace ElearningAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
        Task SendCourseEnrollmentEmailAsync(string toEmail, string userName, string courseTitle);
        Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
        Task SendQuizResultEmailAsync(string toEmail, string userName, string quizTitle, bool passed, int score);
        Task SendForumNotificationEmailAsync(string toEmail, string userName, string postTitle, string authorName);
    }
}