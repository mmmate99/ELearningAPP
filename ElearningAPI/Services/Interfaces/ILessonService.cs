using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Services.Interfaces
{
    public interface ILessonService
    {
        Task<LessonDto?> GetLessonByIdAsync(int id);
        Task<IEnumerable<LessonDto>> GetModuleLessonsAsync(int moduleId);
        Task<LessonDto> CreateLessonAsync(CreateLessonDto createDto);
        Task<LessonDto?> UpdateLessonAsync(int id, UpdateLessonDto updateDto);
        Task<bool> DeleteLessonAsync(int id);
    }
}