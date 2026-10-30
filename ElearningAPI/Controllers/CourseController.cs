using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IProgressService _progressService;

        public CoursesController(ICourseService courseService, IProgressService progressService)
        {
            _courseService = courseService;
            _progressService = progressService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var courses = await _courseService.GetAllCoursesAsync(userId);
            return Ok(courses);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var course = await _courseService.GetCourseByIdAsync(id, userId);
            
            if (course == null)
                return NotFound();
                
            return Ok(course);
        }

        [HttpGet("{id}/details")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseDetailsDto>> GetCourseDetails(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var courseDetails = await _courseService.GetCourseDetailsAsync(id, userId);
            
            if (courseDetails == null)
                return NotFound();
                
            return Ok(courseDetails);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto createDto)
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var course = await _courseService.CreateCourseAsync(createDto, teacherId!);
            
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<CourseDto>> UpdateCourse(int id, UpdateCourseDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var updatedCourse = await _courseService.UpdateCourseAsync(id, updateDto, userId!);
            
            if (updatedCourse == null)
                return NotFound();
                
            return Ok(updatedCourse);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _courseService.DeleteCourseAsync(id, userId!);
            
            if (!success)
                return NotFound();
                
            return NoContent();
        }

        [HttpPost("{id}/enroll")]
        public async Task<ActionResult> EnrollInCourse(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _courseService.EnrollUserAsync(id, userId!);
            
            if (!success)
                return BadRequest(new { message = "Failed to enroll in course" });
                
            return Ok(new { message = "Successfully enrolled in course" });
        }

        [HttpPost("{id}/unenroll")]
        public async Task<ActionResult> UnenrollFromCourse(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _courseService.UnenrollUserAsync(id, userId!);
            
            if (!success)
                return BadRequest(new { message = "Failed to unenroll from course" });
                
            return Ok(new { message = "Successfully unenrolled from course" });
        }

        [HttpPost("{id}/rate")]
        public async Task<ActionResult> RateCourse(int id, [FromBody] CourseRatingDto ratingDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _courseService.UpdateCourseRatingAsync(id, userId!, ratingDto);
            
            if (!success)
                return BadRequest(new { message = "Failed to rate course" });
                
            return Ok(new { message = "Course rated successfully" });
        }

        [HttpGet("{id}/progress")]
        public async Task<ActionResult> GetCourseProgress(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var progress = await _progressService.GetCourseProgressAsync(id, userId!);
            
            return Ok(progress);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseDto>>> SearchCourses(
            [FromQuery] string? q, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var courses = await _courseService.SearchCoursesAsync(q ?? string.Empty, page, pageSize, userId);
            
            return Ok(courses);
        }

        [HttpGet("my-courses")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var courses = await _courseService.GetEnrolledCoursesAsync(userId!);
            
            return Ok(courses);
        }

        [HttpGet("teaching")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetTeachingCourses()
        {
            var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var courses = await _courseService.GetTeacherCoursesAsync(teacherId!);
            
            return Ok(courses);
        }
    }
}