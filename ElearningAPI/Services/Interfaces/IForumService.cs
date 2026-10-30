using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Services.Interfaces
{
    public interface IForumService
    {
        Task<ForumPostDto> CreatePostAsync(CreateForumPostDto createDto, string userId);
        Task<ForumPostDto?> GetPostByIdAsync(int postId);
        Task<IEnumerable<ForumPostDto>> GetCoursePostsAsync(int courseId);
        Task<ForumPostDto> UpdatePostAsync(int postId, UpdateForumPostDto updateDto);
        Task<bool> DeletePostAsync(int postId);
        Task<bool> TogglePinPostAsync(int postId);
        Task<IEnumerable<ForumPostDto>> SearchPostsAsync(string? searchTerm, int? courseId);
        Task<IEnumerable<ForumPostDto>> GetUserPostsAsync(string userId);
    }
}