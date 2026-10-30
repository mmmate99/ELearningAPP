using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IFileService fileService, ILogger<FilesController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FileUploadResponse>> UploadFile(
            IFormFile file,
            [FromForm] FileType fileType = FileType.Document)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var filePath = await _fileService.UploadFileAsync(file, userId!, fileType);
                var fileUrl = await _fileService.GetFileUrlAsync(filePath);

                return Ok(new FileUploadResponse
                {
                    FilePath = filePath,
                    FileUrl = fileUrl,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{*filePath}")]
        public async Task<ActionResult> DeleteFile(string filePath)
        {
            try
            {
                var success = await _fileService.DeleteFileAsync(filePath);
                if (!success)
                    return NotFound();

                return Ok(new { message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("external-video")]
        public async Task<ActionResult<string>> ProcessExternalVideo([FromBody] ExternalVideoRequest request)
        {
            try
            {
                var embedUrl = await _fileService.HandleExternalVideoAsync(request.VideoUrl);
                return Ok(new { embedUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing external video");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("allowed-extensions/{fileType}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllowedExtensions(FileType fileType)
        {
            var extensions = await _fileService.GetAllowedExtensionsAsync(fileType);
            return Ok(extensions);
        }
    }

    public class FileUploadResponse
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }

    public class ExternalVideoRequest
    {
        public string VideoUrl { get; set; } = string.Empty;
    }
}