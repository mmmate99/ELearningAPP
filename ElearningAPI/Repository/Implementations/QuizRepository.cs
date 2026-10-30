using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Repository.Interfaces;

namespace ElearningAPI.Repository.Implementations
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Quiz?> GetByIdAsync(int id)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .Include(q => q.QuizAttempts)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Quizzes
                .Where(q => q.CourseId == courseId && q.IsPublished)
                .Include(q => q.Questions)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetByLessonIdAsync(int lessonId)
        {
            return await _context.Quizzes
                .Where(q => q.LessonId == lessonId && q.IsPublished)
                .Include(q => q.Questions)
                .ToListAsync();
        }

        public async Task<Quiz> CreateAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<Quiz> UpdateAsync(Quiz quiz)
        {
            _context.Entry(quiz).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return false;

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Question>> GetQuizQuestionsAsync(int quizId)
        {
            return await _context.Questions
                .Where(q => q.QuizId == quizId)
                .Include(q => q.AnswerOptions)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }

        public async Task<QuizAttempt?> GetQuizAttemptAsync(string userId, int quizId, int attemptNumber)
        {
            return await _context.QuizAttempts
                .FirstOrDefaultAsync(qa => qa.UserId == userId && 
                                         qa.QuizId == quizId && 
                                         qa.AttemptNumber == attemptNumber);
        }

        public async Task<IEnumerable<QuizAttempt>> GetUserQuizAttemptsAsync(string userId, int quizId)
        {
            return await _context.QuizAttempts
                .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
                .OrderByDescending(qa => qa.StartedAt)
                .ToListAsync();
        }

        public async Task<QuizAttempt> CreateQuizAttemptAsync(QuizAttempt attempt)
        {
            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<int> GetNextAttemptNumberAsync(string userId, int quizId)
        {
            var lastAttempt = await _context.QuizAttempts
                .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
                .OrderByDescending(qa => qa.AttemptNumber)
                .FirstOrDefaultAsync();
            
            return (lastAttempt?.AttemptNumber ?? 0) + 1;
        }
    }
}