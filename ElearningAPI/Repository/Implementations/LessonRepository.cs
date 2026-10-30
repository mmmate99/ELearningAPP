using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Repository.Interfaces;

namespace ElearningAPI.Repository.Implementations
{
    public class LessonRepository : ILessonRepository
    {
        private readonly ApplicationDbContext _context;

        public LessonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Lesson?> GetByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(l => l.Module)
                    .ThenInclude(m => m.Course)
                .Include(l => l.Quizzes)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Lesson>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.Lessons
                .Where(l => l.ModuleId == moduleId)
                .OrderBy(l => l.Order)
                .ToListAsync();
        }

        public async Task<Lesson> CreateAsync(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<Lesson> UpdateAsync(Lesson lesson)
        {
            _context.Entry(lesson).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return lesson;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return false;

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Lessons.AnyAsync(l => l.Id == id);
        }
    }
}