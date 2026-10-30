using ElearningAPI.Models;

namespace ElearningAPI.Repository.Interfaces
{
    public interface IQuizRepository
    {
        Task<Quiz?> GetByIdAsync(int id);
        Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId);
        Task<IEnumerable<Quiz>> GetByLessonIdAsync(int lessonId);
        Task<Quiz> CreateAsync(Quiz quiz);
        Task<Quiz> UpdateAsync(Quiz quiz);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Question>> GetQuizQuestionsAsync(int quizId);
        Task<QuizAttempt?> GetQuizAttemptAsync(string userId, int quizId, int attemptNumber);
        Task<IEnumerable<QuizAttempt>> GetUserQuizAttemptsAsync(string userId, int quizId);
        Task<QuizAttempt> CreateQuizAttemptAsync(QuizAttempt attempt);
        Task<int> GetNextAttemptNumberAsync(string userId, int quizId);
    }
}