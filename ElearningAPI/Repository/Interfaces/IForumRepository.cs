using ElearningAPI.Models;

namespace ElearningAPI.Repository.Interfaces
{
    public interface IForumRepository
    {
        Task<ForumPost> CreateAsync(ForumPost post);
        Task<ForumPost?> GetByIdAsync(int id);
        Task<IEnumerable<ForumPost>> GetByCourseIdAsync(int courseId);
        Task<ForumPost> UpdateAsync(ForumPost post);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ForumPost>> SearchAsync(string? searchTerm, int? courseId);
        Task<IEnumerable<ForumPost>> GetByUserIdAsync(string userId);
    }
}