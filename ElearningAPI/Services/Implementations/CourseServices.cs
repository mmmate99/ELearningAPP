using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly IProgressRepository _progressRepository;
        private readonly ApplicationDbContext _context;

        public CourseService(
            ICourseRepository courseRepository,
            IMapper mapper,
            IProgressRepository progressRepository,
            ApplicationDbContext context)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
            _progressRepository = progressRepository;
            _context = context;
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id, string? userId = null)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return null;

            var courseDto = _mapper.Map<CourseDto>(course);
            
            if (!string.IsNullOrEmpty(userId))
            {
                courseDto.IsEnrolled = await _courseRepository.IsUserEnrolledAsync(userId, id);
                if (courseDto.IsEnrolled)
                {
                    var enrollment = await _courseRepository.GetEnrollmentAsync(userId, id);
                    courseDto.UserRating = enrollment?.Rating;
                }
            }
            
            return courseDto;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync(string? userId = null)
        {
            var courses = await _courseRepository.GetAllAsync();
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var courseDto in courseDtos)
                {
                    courseDto.IsEnrolled = await _courseRepository.IsUserEnrolledAsync(userId, courseDto.Id);
                }
            }

            return courseDtos;
        }

        public async Task<IEnumerable<CourseDto>> GetTeacherCoursesAsync(string teacherId)
        {
            var courses = await _courseRepository.GetByTeacherIdAsync(teacherId);
            return _mapper.Map<List<CourseDto>>(courses);
        }

        public async Task<IEnumerable<CourseDto>> GetEnrolledCoursesAsync(string userId)
        {
            var courses = await _courseRepository.GetEnrolledCoursesAsync(userId);
            return _mapper.Map<List<CourseDto>>(courses);
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createDto, string teacherId)
        {
            var course = _mapper.Map<Course>(createDto);
            course.TeacherId = teacherId;
            course.CreatedAt = DateTime.UtcNow;
            course.IsPublished = false; // Alapértelmezetten nem publikált

            var createdCourse = await _courseRepository.CreateAsync(course);
            return _mapper.Map<CourseDto>(createdCourse);
        }

        public async Task<CourseDto?> UpdateCourseAsync(int id, UpdateCourseDto updateDto, string userId)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return null;

            // Ellenőrizd, hogy a felhasználó a kurzus tanára-e vagy admin
            if (course.TeacherId != userId && !await IsUserAdminAsync(userId))
            {
                throw new UnauthorizedAccessException("Only the course teacher or admin can update the course");
            }

            _mapper.Map(updateDto, course);
            course.UpdatedAt = DateTime.UtcNow;

            var updatedCourse = await _courseRepository.UpdateAsync(course);
            return _mapper.Map<CourseDto>(updatedCourse);
        }

        public async Task<bool> DeleteCourseAsync(int id, string userId)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) return false;

            // Ellenőrizd, hogy a felhasználó a kurzus tanára-e vagy admin
            if (course.TeacherId != userId && !await IsUserAdminAsync(userId))
            {
                throw new UnauthorizedAccessException("Only the course teacher or admin can delete the course");
            }

            return await _courseRepository.DeleteAsync(id);
        }

        public async Task<bool> EnrollUserAsync(int courseId, string userId)
        {
            // Ellenőrizd, hogy a kurzus létezik-e
            if (!await _courseRepository.ExistsAsync(courseId))
                return false;

            // Ellenőrizd, hogy már fel van-e iratkozva
            if (await _courseRepository.IsUserEnrolledAsync(userId, courseId))
                return true; // Már feliratkozott, ez nem hiba

            await _courseRepository.EnrollUserAsync(userId, courseId);
            return true;
        }

        public async Task<bool> UnenrollUserAsync(int courseId, string userId)
        {
            var enrollment = await _courseRepository.GetEnrollmentAsync(userId, courseId);
            if (enrollment == null) return false;

            enrollment.IsActive = false;
            await _courseRepository.UpdateEnrollmentAsync(enrollment);
            return true;
        }

        public async Task<bool> UpdateCourseRatingAsync(int courseId, string userId, CourseRatingDto ratingDto)
        {
            var enrollment = await _courseRepository.GetEnrollmentAsync(userId, courseId);
            if (enrollment == null) return false;

            enrollment.Rating = ratingDto.Rating;
            enrollment.Review = ratingDto.Review;
            enrollment.ReviewedAt = DateTime.UtcNow;

            await _courseRepository.UpdateEnrollmentAsync(enrollment);
            return true;
        }

        public async Task<CourseDetailsDto?> GetCourseDetailsAsync(int id, string? userId = null)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                .Include(c => c.Quizzes)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(q => q.AnswerOptions)
                .Include(c => c.ForumPosts.Where(fp => fp.ParentPostId == null))
                    .ThenInclude(fp => fp.Replies)
                .Include(c => c.UserCourses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return null;

            var courseDetails = _mapper.Map<CourseDetailsDto>(course);

            if (!string.IsNullOrEmpty(userId))
            {
                courseDetails.IsEnrolled = await _courseRepository.IsUserEnrolledAsync(userId, id);
                
                if (courseDetails.IsEnrolled)
                {
                    // Progress adatok hozzáadása minden leckéhez
                    foreach (var module in courseDetails.Modules)
                    {
                        foreach (var lesson in module.Lessons)
                        {
                            var progress = await _progressRepository.GetUserProgressAsync(userId, lesson.Id);
                            if (progress != null)
                            {
                                lesson.IsCompleted = progress.IsCompleted;
                                lesson.TimeSpent = progress.TimeSpent;
                                lesson.CurrentTime = progress.CurrentTime;
                            }
                        }
                    }
                }
            }

            return courseDetails;
        }

        public async Task<IEnumerable<CourseDto>> SearchCoursesAsync(string searchTerm, int page = 1, int pageSize = 10, string? userId = null)
        {
            var courses = await _courseRepository.SearchAsync(searchTerm, page, pageSize);
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);

            if (!string.IsNullOrEmpty(userId))
            {
                foreach (var courseDto in courseDtos)
                {
                    courseDto.IsEnrolled = await _courseRepository.IsUserEnrolledAsync(userId, courseDto.Id);
                }
            }

            return courseDtos;
        }

        private async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Role == UserRole.Admin;
        }

        public async Task<bool> IsUserEnrolledAsync(int courseId, string userId)
        {
            return await _courseRepository.IsUserEnrolledAsync(userId, courseId);
        }
    }
}