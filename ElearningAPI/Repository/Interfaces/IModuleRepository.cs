using ElearningAPI.Models;

namespace ElearningAPI.Repository.Interfaces
{
    public interface IModuleRepository
    {
        Task<Module?> GetByIdAsync(int id);
        Task<IEnumerable<Module>> GetByCourseIdAsync(int courseId);
        Task<Module> CreateAsync(Module module);
        Task<Module> UpdateAsync(Module module);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}