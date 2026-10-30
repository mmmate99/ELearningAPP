using ElearningAPI.Models;

namespace ElearningAPI.Repository.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> GetByTeacherIdAsync(string teacherId);
        Task<IEnumerable<Course>> GetEnrolledCoursesAsync(string userId);
        Task<IEnumerable<Course>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<Course> CreateAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetStudentCountAsync(int courseId);
        Task<UserCourse?> GetEnrollmentAsync(string userId, int courseId);
        Task<UserCourse> EnrollUserAsync(string userId, int courseId);
        Task<UserCourse> UpdateEnrollmentAsync(UserCourse enrollment);
        Task<bool> IsUserEnrolledAsync(string userId, int courseId);
        Task<IEnumerable<UserCourse>> GetCourseEnrollmentsAsync(int courseId);
    }
}