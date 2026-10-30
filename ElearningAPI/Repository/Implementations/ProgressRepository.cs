using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Repository.Interfaces;

namespace ElearningAPI.Repository.Implementations
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public ProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Progress?> GetUserProgressAsync(string userId, int lessonId)
        {
            return await _context.Progresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);
        }

        public async Task<IEnumerable<Progress>> GetUserCourseProgressAsync(string userId, int courseId)
        {
            return await _context.Progresses
                .Include(p => p.Lesson)
                    .ThenInclude(l => l.Module)
                .Where(p => p.UserId == userId && p.Lesson.Module.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Progress> CreateOrUpdateProgressAsync(Progress progress)
        {
            var existing = await GetUserProgressAsync(progress.UserId, progress.LessonId);
            
            if (existing != null)
            {
                existing.IsCompleted = progress.IsCompleted;
                existing.TimeSpent = progress.TimeSpent;
                existing.CurrentTime = progress.CurrentTime;
                existing.LastAccessed = DateTime.UtcNow;
                
                if (progress.IsCompleted && !existing.IsCompleted)
                {
                    existing.CompletedAt = DateTime.UtcNow;
                }
                
                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return existing;
            }
            else
            {
                progress.LastAccessed = DateTime.UtcNow;
                _context.Progresses.Add(progress);
                await _context.SaveChangesAsync();
                return progress;
            }
        }

        public async Task<bool> MarkLessonCompletedAsync(string userId, int lessonId)
        {
            var progress = await GetUserProgressAsync(userId, lessonId);
            
            if (progress == null)
            {
                progress = new Progress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    LastAccessed = DateTime.UtcNow
                };
                _context.Progresses.Add(progress);
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
                progress.LastAccessed = DateTime.UtcNow;
                _context.Entry(progress).State = EntityState.Modified;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<double> GetCourseCompletionPercentageAsync(string userId, int courseId)
        {
            var totalLessons = await _context.Lessons
                .Where(l => l.Module.CourseId == courseId)
                .CountAsync();
            
            if (totalLessons == 0) return 0;
            
            var completedLessons = await _context.Progresses
                .CountAsync(p => p.UserId == userId && 
                               p.IsCompleted && 
                               p.Lesson.Module.CourseId == courseId);
            
            return (double)completedLessons / totalLessons * 100;
        }

        public async Task<IEnumerable<Progress>> GetRecentProgressAsync(string userId, int count = 10)
        {
            return await _context.Progresses
                .Where(p => p.UserId == userId)
                .Include(p => p.Lesson)
                .OrderByDescending(p => p.LastAccessed)
                .Take(count)
                .ToListAsync();
        }
    }
}