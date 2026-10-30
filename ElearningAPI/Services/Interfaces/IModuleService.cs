using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Services.Interfaces
{
    public interface IModuleService
    {
        Task<ModuleDto?> GetModuleByIdAsync(int id);
        Task<IEnumerable<ModuleDto>> GetCourseModulesAsync(int courseId);
        Task<ModuleDto> CreateModuleAsync(CreateModuleDto createDto);
        Task<ModuleDto?> UpdateModuleAsync(int id, CreateModuleDto updateDto);
        Task<bool> DeleteModuleAsync(int id);
    }
}