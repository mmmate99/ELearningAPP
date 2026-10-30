using AutoMapper;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressRepository _progressRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IMapper _mapper;

        public ProgressService(
            IProgressRepository progressRepository,
            ILessonRepository lessonRepository,
            IMapper mapper)
        {
            _progressRepository = progressRepository;
            _lessonRepository = lessonRepository;
            _mapper = mapper;
        }

        public async Task<LessonDto?> GetLessonWithProgressAsync(int lessonId, string userId)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null) return null;

            var lessonDto = _mapper.Map<LessonDto>(lesson);
            var progress = await _progressRepository.GetUserProgressAsync(userId, lessonId);
            
            if (progress != null)
            {
                lessonDto.IsCompleted = progress.IsCompleted;
                lessonDto.TimeSpent = progress.TimeSpent;
                lessonDto.CurrentTime = progress.CurrentTime;
            }

            // Navigációs információk hozzáadása - ÚJ
            lessonDto.ModuleTitle = lesson.Module?.Title;
            
            // Előző és következő lecke megkeresése
            var moduleLessons = await _lessonRepository.GetByModuleIdAsync(lesson.ModuleId);
            var orderedLessons = moduleLessons.OrderBy(l => l.Order).ToList();
            var currentIndex = orderedLessons.FindIndex(l => l.Id == lessonId);
            
            if (currentIndex > 0)
            {
                lessonDto.PreviousLessonId = orderedLessons[currentIndex - 1].Id;
            }
            
            if (currentIndex >= 0 && currentIndex < orderedLessons.Count - 1)
            {
                lessonDto.NextLessonId = orderedLessons[currentIndex + 1].Id;
            }

            return lessonDto;
        }

        public async Task<ProgressDto> UpdateLessonProgressAsync(int lessonId, string userId, UpdateProgressDto progressDto)
        {
            var progress = new Progress
            {
                UserId = userId,
                LessonId = lessonId,
                IsCompleted = progressDto.IsCompleted ?? false,
                TimeSpent = progressDto.TimeSpent ?? 0,
                CurrentTime = progressDto.CurrentTime ?? 0,
                LastAccessed = DateTime.UtcNow
            };

            var updatedProgress = await _progressRepository.CreateOrUpdateProgressAsync(progress);
            return _mapper.Map<ProgressDto>(updatedProgress);
        }

        public async Task<bool> MarkLessonAsCompletedAsync(int lessonId, string userId)
        {
            return await _progressRepository.MarkLessonCompletedAsync(userId, lessonId);
        }

        public async Task<double> GetCourseCompletionPercentageAsync(int courseId, string userId)
        {
            return await _progressRepository.GetCourseCompletionPercentageAsync(userId, courseId);
        }

        public async Task<IEnumerable<ProgressDto>> GetCourseProgressAsync(int courseId, string userId)
        {
            var progress = await _progressRepository.GetUserCourseProgressAsync(userId, courseId);
            return _mapper.Map<List<ProgressDto>>(progress);
        }

        public async Task<IEnumerable<LessonDto>> GetRecentLessonsAsync(string userId, int count = 5)
        {
            var recentProgress = await _progressRepository.GetRecentProgressAsync(userId, count);
            var lessons = new List<LessonDto>();

            foreach (var progress in recentProgress)
            {
                var lesson = await _lessonRepository.GetByIdAsync(progress.LessonId);
                if (lesson != null)
                {
                    var lessonDto = _mapper.Map<LessonDto>(lesson);
                    lessonDto.IsCompleted = progress.IsCompleted;
                    lessonDto.TimeSpent = progress.TimeSpent;
                    lessonDto.CurrentTime = progress.CurrentTime;
                    lessons.Add(lessonDto);
                }
            }

            return lessons;
        }
    }
}