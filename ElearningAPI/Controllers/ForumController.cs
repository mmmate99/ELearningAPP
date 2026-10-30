using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;
        private readonly ICourseService _courseService;

        public ForumController(IForumService forumService, ICourseService courseService)
        {
            _forumService = forumService;
            _courseService = courseService;
        }

        [HttpGet("courses/{courseId}")]
        public async Task<ActionResult<IEnumerable<ForumPostDto>>> GetCoursePosts(int courseId)
        {
            // Ellenőrizd, hogy a felhasználó fel van-e iratkozva a kurzusra
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isEnrolled = await _courseService.IsUserEnrolledAsync(courseId, userId!);
            
            if (!isEnrolled && !User.IsInRole("Teacher") && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var posts = await _forumService.GetCoursePostsAsync(courseId);
            return Ok(posts);
        }

        [HttpGet("{postId}")]
        public async Task<ActionResult<ForumPostDto>> GetPost(int postId)
        {
            var post = await _forumService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();

            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<ForumPostDto>> CreatePost(CreateForumPostDto createDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Ellenőrizd, hogy a felhasználó fel van-e iratkozva a kurzusra
            var isEnrolled = await _courseService.IsUserEnrolledAsync(createDto.CourseId, userId!);
            
            if (!isEnrolled && !User.IsInRole("Teacher") && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var post = await _forumService.CreatePostAsync(createDto, userId!);
            return CreatedAtAction(nameof(GetPost), new { postId = post.Id }, post);
        }

        [HttpPut("{postId}")]
        public async Task<ActionResult<ForumPostDto>> UpdatePost(int postId, UpdateForumPostDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = await _forumService.GetPostByIdAsync(postId);
            
            if (post == null)
                return NotFound();

            // Csak a post szerzője vagy admin módosíthat
            if (post.Author.Id != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var updatedPost = await _forumService.UpdatePostAsync(postId, updateDto);
            return Ok(updatedPost);
        }

        [HttpDelete("{postId}")]
        public async Task<ActionResult> DeletePost(int postId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var post = await _forumService.GetPostByIdAsync(postId);
            
            if (post == null)
                return NotFound();

            // Csak a post szerzője vagy admin törölhet
            if (post.Author.Id != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _forumService.DeletePostAsync(postId);
            return NoContent();
        }

        [HttpPost("{postId}/pin")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult> TogglePinPost(int postId)
        {
            await _forumService.TogglePinPostAsync(postId);
            return Ok(new { message = "Post pin status updated" });
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ForumPostDto>>> SearchPosts(
            [FromQuery] string? q,
            [FromQuery] int? courseId)
        {
            var posts = await _forumService.SearchPostsAsync(q, courseId);
            return Ok(posts);
        }

        [HttpGet("my-posts")]
        public async Task<ActionResult<IEnumerable<ForumPostDto>>> GetMyPosts()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var posts = await _forumService.GetUserPostsAsync(userId!);
            return Ok(posts);
        }
    }
}