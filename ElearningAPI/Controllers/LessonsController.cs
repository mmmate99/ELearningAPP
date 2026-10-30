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
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly IProgressService _progressService;

        public LessonsController(ILessonService lessonService, IProgressService progressService)
        {
            _lessonService = lessonService;
            _progressService = progressService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDto>> GetLesson(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var lesson = await _progressService.GetLessonWithProgressAsync(id, userId!);
            
            if (lesson == null)
                return NotFound();
                
            return Ok(lesson);
        }

        [HttpPost("{id}/progress")]
        public async Task<ActionResult<ProgressDto>> UpdateProgress(int id, [FromBody] UpdateProgressDto progressDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var progress = await _progressService.UpdateLessonProgressAsync(id, userId!, progressDto);
            return Ok(progress);
        }

        [HttpPost("{id}/complete")]
        public async Task<ActionResult> MarkAsCompleted(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var success = await _progressService.MarkLessonAsCompletedAsync(id, userId!);
            
            if (!success)
                return BadRequest(new { message = "Failed to mark lesson as completed" });
                
            return Ok(new { message = "Lesson marked as completed" });
        }

        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetRecentLessons()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var lessons = await _progressService.GetRecentLessonsAsync(userId!);
            
            return Ok(lessons);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<LessonDto>> CreateLesson(CreateLessonDto createDto)
        {
            var lesson = await _lessonService.CreateLessonAsync(createDto);
            return CreatedAtAction(nameof(GetLesson), new { id = lesson.Id }, lesson);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<LessonDto>> UpdateLesson(int id, UpdateLessonDto updateDto)
        {
            var lesson = await _lessonService.UpdateLessonAsync(id, updateDto);
            if (lesson == null)
                return NotFound();
            return Ok(lesson);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult> DeleteLesson(int id)
        {
            var success = await _lessonService.DeleteLessonAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}