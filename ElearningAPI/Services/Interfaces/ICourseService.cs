using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto?> GetCourseByIdAsync(int id, string? userId = null);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync(string? userId = null);
        Task<IEnumerable<CourseDto>> GetTeacherCoursesAsync(string teacherId);
        Task<IEnumerable<CourseDto>> GetEnrolledCoursesAsync(string userId);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createDto, string teacherId);
        Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto updateDto, string userId);
        Task<bool> DeleteCourseAsync(int id, string userId);
        Task<bool> EnrollUserAsync(int courseId, string userId);
        Task<bool> UnenrollUserAsync(int courseId, string userId);
        Task<bool> UpdateCourseRatingAsync(int courseId, string userId, CourseRatingDto ratingDto);
        Task<CourseDetailsDto?> GetCourseDetailsAsync(int id, string? userId = null);
        Task<IEnumerable<CourseDto>> SearchCoursesAsync(string searchTerm, int page = 1, int pageSize = 10, string? userId = null);
        Task<bool> IsUserEnrolledAsync(int courseId, string userId);
    }
}