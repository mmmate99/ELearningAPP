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
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModuleDto>> GetModule(int id)
        {
            var module = await _moduleService.GetModuleByIdAsync(id);
            if (module == null)
                return NotFound();
            return Ok(module);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<ModuleDto>> CreateModule(CreateModuleDto createDto)
        {
            var module = await _moduleService.CreateModuleAsync(createDto);
            return CreatedAtAction(nameof(GetModule), new { id = module.Id }, module);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<ModuleDto>> UpdateModule(int id, CreateModuleDto updateDto)
        {
            var module = await _moduleService.UpdateModuleAsync(id, updateDto);
            if (module == null)
                return NotFound();
            return Ok(module);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult> DeleteModule(int id)
        {
            var success = await _moduleService.DeleteModuleAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}