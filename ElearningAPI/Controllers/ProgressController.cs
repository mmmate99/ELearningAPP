using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ElearningAPI.Services.Interfaces;
using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProgressController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboardProgress()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            var recentLessons = await _progressService.GetRecentLessonsAsync(userId!, 5);
            
            return Ok(new 
            {
                recentLessons,
                totalTimeSpent = recentLessons.Sum(l => l.TimeSpent ?? 0),
                completedLessons = recentLessons.Count(l => l.IsCompleted)
            });
        }

        [HttpPost("batch")]
        public async Task<ActionResult> BatchUpdateProgress([FromBody] List<BatchProgressDto> updates)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            foreach (var update in updates)
            {
                await _progressService.UpdateLessonProgressAsync(
                    update.LessonId, 
                    userId!, 
                    new UpdateProgressDto 
                    {
                        TimeSpent = update.TimeSpent,
                        CurrentTime = update.CurrentTime,
                        IsCompleted = update.IsCompleted
                    });
            }
            
            return Ok(new { message = "Progress updated successfully" });
        }
    }

    public class BatchProgressDto
    {
        public int LessonId { get; set; }
        public int? TimeSpent { get; set; }
        public int? CurrentTime { get; set; }
        public bool? IsCompleted { get; set; }
    }
}