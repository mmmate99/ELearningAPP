using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Repository.Interfaces;

namespace ElearningAPI.Repository.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .Include(c => c.UserCourses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.UserCourses)
                .Where(c => c.IsPublished)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByTeacherIdAsync(string teacherId)
        {
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.UserCourses)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetEnrolledCoursesAsync(string userId)
        {
            return await _context.UserCourses
                .Where(uc => uc.UserId == userId && uc.IsActive)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Teacher)
                .Include(uc => uc.Course)
                    .ThenInclude(c => c.Modules)
                .Select(uc => uc.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> SearchAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            var query = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.UserCourses)
                .Where(c => c.IsPublished);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => 
                    c.Title.Contains(searchTerm) || 
                    c.Description.Contains(searchTerm) ||
                    c.Teacher.FirstName.Contains(searchTerm) ||
                    c.Teacher.LastName.Contains(searchTerm));
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Course> CreateAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetStudentCountAsync(int courseId)
        {
            return await _context.UserCourses
                .CountAsync(uc => uc.CourseId == courseId && uc.IsActive);
        }

        public async Task<UserCourse?> GetEnrollmentAsync(string userId, int courseId)
        {
            return await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        }

        public async Task<UserCourse> EnrollUserAsync(string userId, int courseId)
        {
            var enrollment = new UserCourse
            {
                UserId = userId,
                CourseId = courseId,
                EnrolledAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.UserCourses.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<UserCourse> UpdateEnrollmentAsync(UserCourse enrollment)
        {
            _context.Entry(enrollment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> IsUserEnrolledAsync(string userId, int courseId)
        {
            return await _context.UserCourses
                .AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId && uc.IsActive);
        }

        public async Task<IEnumerable<UserCourse>> GetCourseEnrollmentsAsync(int courseId)
        {
            return await _context.UserCourses
                .Where(uc => uc.CourseId == courseId)
                .Include(uc => uc.User)
                .ToListAsync();
        }
    }
}