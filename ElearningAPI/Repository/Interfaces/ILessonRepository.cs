using ElearningAPI.Models;

namespace ElearningAPI.Repository.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetByIdAsync(int id);
        Task<IEnumerable<Lesson>> GetByModuleIdAsync(int moduleId);
        Task<Lesson> CreateAsync(Lesson lesson);
        Task<Lesson> UpdateAsync(Lesson lesson);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}