using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Repository.Interfaces;

namespace ElearningAPI.Repository.Implementations
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly ApplicationDbContext _context;

        public ModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            return await _context.Modules
                .Include(m => m.Lessons)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Module>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Modules
                .Where(m => m.CourseId == courseId)
                .Include(m => m.Lessons)
                .OrderBy(m => m.Order)
                .ToListAsync();
        }

        public async Task<Module> CreateAsync(Module module)
        {
            _context.Modules.Add(module);
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<Module> UpdateAsync(Module module)
        {
            _context.Entry(module).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return module;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            if (module == null) return false;

            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Modules.AnyAsync(m => m.Id == id);
        }
    }
}