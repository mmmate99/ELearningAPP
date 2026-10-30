using Microsoft.EntityFrameworkCore;
using ElearningAPI.Data;
using ElearningAPI.Models;
using ElearningAPI.Repository.Interfaces;

namespace ElearningAPI.Repository.Implementations
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _context;

        public ForumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ForumPost> CreateAsync(ForumPost post)
        {
            _context.ForumPosts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<ForumPost?> GetByIdAsync(int id)
        {
            return await _context.ForumPosts
                .Include(fp => fp.User)
                .Include(fp => fp.Replies)
                    .ThenInclude(r => r.User)
                .Include(fp => fp.Course)
                .FirstOrDefaultAsync(fp => fp.Id == id);
        }

        public async Task<IEnumerable<ForumPost>> GetByCourseIdAsync(int courseId)
        {
            return await _context.ForumPosts
                .Where(fp => fp.CourseId == courseId && fp.ParentPostId == null)
                .Include(fp => fp.User)
                .Include(fp => fp.Replies)
                    .ThenInclude(r => r.User)
                .OrderByDescending(fp => fp.IsPinned)
                .ThenByDescending(fp => fp.CreatedAt)
                .ToListAsync();
        }

        public async Task<ForumPost> UpdateAsync(ForumPost post)
        {
            post.UpdatedAt = DateTime.UtcNow;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var post = await _context.ForumPosts
                .Include(fp => fp.Replies)
                .FirstOrDefaultAsync(fp => fp.Id == id);
                
            if (post == null) return false;

            // Töröld a válaszokat is
            _context.ForumPosts.RemoveRange(post.Replies);
            _context.ForumPosts.Remove(post);
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ForumPost>> SearchAsync(string? searchTerm, int? courseId)
        {
            var query = _context.ForumPosts
                .Include(fp => fp.User)
                .Include(fp => fp.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(fp => fp.CourseId == courseId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(fp => 
                    fp.Title.Contains(searchTerm) || 
                    fp.Content.Contains(searchTerm));
            }

            return await query
                .OrderByDescending(fp => fp.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task<IEnumerable<ForumPost>> GetByUserIdAsync(string userId)
        {
            return await _context.ForumPosts
                .Where(fp => fp.UserId == userId)
                .Include(fp => fp.Course)
                .OrderByDescending(fp => fp.CreatedAt)
                .ToListAsync();
        }
    }
}