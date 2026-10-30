using AutoMapper;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;
        private readonly IMapper _mapper;

        public ForumService(IForumRepository forumRepository, IMapper mapper)
        {
            _forumRepository = forumRepository;
            _mapper = mapper;
        }

        public async Task<ForumPostDto> CreatePostAsync(CreateForumPostDto createDto, string userId)
        {
            var post = _mapper.Map<ForumPost>(createDto);
            post.UserId = userId;
            post.CreatedAt = DateTime.UtcNow;

            var createdPost = await _forumRepository.CreateAsync(post);
            return _mapper.Map<ForumPostDto>(createdPost);
        }

        public async Task<ForumPostDto?> GetPostByIdAsync(int postId)
        {
            var post = await _forumRepository.GetByIdAsync(postId);
            return post != null ? _mapper.Map<ForumPostDto>(post) : null;
        }

        public async Task<IEnumerable<ForumPostDto>> GetCoursePostsAsync(int courseId)
        {
            var posts = await _forumRepository.GetByCourseIdAsync(courseId);
            return _mapper.Map<List<ForumPostDto>>(posts);
        }

        public async Task<ForumPostDto> UpdatePostAsync(int postId, UpdateForumPostDto updateDto)
        {
            var post = await _forumRepository.GetByIdAsync(postId);
            if (post == null)
                throw new KeyNotFoundException($"Post with id {postId} not found");

            _mapper.Map(updateDto, post);
            post.UpdatedAt = DateTime.UtcNow;

            var updatedPost = await _forumRepository.UpdateAsync(post);
            return _mapper.Map<ForumPostDto>(updatedPost);
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            return await _forumRepository.DeleteAsync(postId);
        }

        public async Task<bool> TogglePinPostAsync(int postId)
        {
            var post = await _forumRepository.GetByIdAsync(postId);
            if (post == null) return false;

            post.IsPinned = !post.IsPinned;
            await _forumRepository.UpdateAsync(post);
            return true;
        }

        public async Task<IEnumerable<ForumPostDto>> SearchPostsAsync(string? searchTerm, int? courseId)
        {
            var posts = await _forumRepository.SearchAsync(searchTerm, courseId);
            return _mapper.Map<List<ForumPostDto>>(posts);
        }

        public async Task<IEnumerable<ForumPostDto>> GetUserPostsAsync(string userId)
        {
            var posts = await _forumRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<ForumPostDto>>(posts);
        }
    }
}