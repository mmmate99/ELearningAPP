using AutoMapper;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository _moduleRepository; // Javítva
        private readonly IMapper _mapper;

        public ModuleService(IModuleRepository moduleRepository, IMapper mapper)
        {
            _moduleRepository = moduleRepository;
            _mapper = mapper;
        }

        public async Task<ModuleDto?> GetModuleByIdAsync(int id)
        {
            var module = await _moduleRepository.GetByIdAsync(id);
            return module != null ? _mapper.Map<ModuleDto>(module) : null;
        }

        public async Task<IEnumerable<ModuleDto>> GetCourseModulesAsync(int courseId)
        {
            var modules = await _moduleRepository.GetByCourseIdAsync(courseId);
            return _mapper.Map<List<ModuleDto>>(modules);
        }

        public async Task<ModuleDto> CreateModuleAsync(CreateModuleDto createDto)
        {
            var module = _mapper.Map<Module>(createDto);
            var createdModule = await _moduleRepository.CreateAsync(module);
            return _mapper.Map<ModuleDto>(createdModule);
        }

        public async Task<ModuleDto?> UpdateModuleAsync(int id, CreateModuleDto updateDto)
        {
            var module = await _moduleRepository.GetByIdAsync(id);
            if (module == null) return null;

            _mapper.Map(updateDto, module);
            var updatedModule = await _moduleRepository.UpdateAsync(module);
            return _mapper.Map<ModuleDto>(updatedModule);
        }

        public async Task<bool> DeleteModuleAsync(int id)
        {
            return await _moduleRepository.DeleteAsync(id);
        }
    }
}