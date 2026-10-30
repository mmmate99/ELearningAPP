using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizDto?> GetQuizByIdAsync(int id);
        Task<QuizDto> CreateQuizAsync(CreateQuizDto createDto);
        Task<QuizDto?> UpdateQuizAsync(int id, CreateQuizDto updateDto);
        Task<bool> DeleteQuizAsync(int id);
        Task<IEnumerable<QuestionDto>> GetQuizQuestionsAsync(int quizId);
        Task<QuizResultDto> SubmitQuizAttemptAsync(int quizId, string userId, SubmitQuizDto submitDto);
        Task<IEnumerable<QuizAttemptDto>> GetUserQuizAttemptsAsync(int quizId, string userId);
        Task<bool> CanUserTakeQuizAsync(int quizId, string userId);
    }
}