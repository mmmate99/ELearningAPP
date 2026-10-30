using AutoMapper;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly IMapper _mapper;

        public LessonService(ILessonRepository lessonRepository, IMapper mapper)
        {
            _lessonRepository = lessonRepository;
            _mapper = mapper;
        }

        public async Task<LessonDto?> GetLessonByIdAsync(int id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            return lesson != null ? _mapper.Map<LessonDto>(lesson) : null;
        }

        public async Task<IEnumerable<LessonDto>> GetModuleLessonsAsync(int moduleId)
        {
            var lessons = await _lessonRepository.GetByModuleIdAsync(moduleId);
            return _mapper.Map<List<LessonDto>>(lessons);
        }

        public async Task<LessonDto> CreateLessonAsync(CreateLessonDto createDto)
        {
            var lesson = _mapper.Map<Lesson>(createDto);
            var createdLesson = await _lessonRepository.CreateAsync(lesson);
            return _mapper.Map<LessonDto>(createdLesson);
        }

        public async Task<LessonDto?> UpdateLessonAsync(int id, UpdateLessonDto updateDto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) return null;

            _mapper.Map(updateDto, lesson);
            var updatedLesson = await _lessonRepository.UpdateAsync(lesson);
            return _mapper.Map<LessonDto>(updatedLesson);
        }

        public async Task<bool> DeleteLessonAsync(int id)
        {
            return await _lessonRepository.DeleteAsync(id);
        }
    }
}