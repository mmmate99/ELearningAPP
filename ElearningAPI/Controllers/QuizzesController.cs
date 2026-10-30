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
    public class QuizzesController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizzesController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpPost("{quizId}/attempt")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuizAttempt(
            int quizId, 
            [FromBody] SubmitQuizDto submitDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _quizService.SubmitQuizAttemptAsync(quizId, userId!, submitDto);
            
            return Ok(result);
        }

        [HttpGet("{quizId}/questions")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuizQuestions(int quizId)
        {
            var questions = await _quizService.GetQuizQuestionsAsync(quizId);
            return Ok(questions);
        }

        [HttpGet("{quizId}/attempts")]
        public async Task<ActionResult<IEnumerable<QuizAttemptDto>>> GetQuizAttempts(int quizId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var attempts = await _quizService.GetUserQuizAttemptsAsync(quizId, userId!);
            return Ok(attempts);
        }
    }
}