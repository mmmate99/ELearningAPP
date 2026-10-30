using Microsoft.Extensions.Configuration;
using ElearningAPI.Services.Interfaces;

namespace ElearningAPI.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileService> _logger;

        public FileService(
            IWebHostEnvironment environment, 
            IConfiguration configuration,
            ILogger<FileService> logger)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string userId, FileType fileType)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            // Méret ellenőrzés
            var maxFileSizeMB = _configuration.GetValue<int>("FileUpload:MaxFileSizeMB", 100);
            if (file.Length > maxFileSizeMB * 1024 * 1024)
                throw new ArgumentException($"File too large. Maximum size is {maxFileSizeMB}MB");

            // Kiterjesztés ellenőrzés
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = await GetAllowedExtensionsAsync(fileType);
            
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}");

            // Mappa létrehozása
            var uploadsFolder = GetUploadFolder(fileType, userId);
            Directory.CreateDirectory(uploadsFolder);

            // Fájlnév generálása
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Fájl mentése
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"File uploaded: {filePath} by user {userId}");
            
            // Relatív útvonal visszaadása
            return GetRelativePath(filePath);
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var absolutePath = GetAbsolutePath(filePath);
            
            if (!File.Exists(absolutePath))
                return false;

            try
            {
                File.Delete(absolutePath);
                _logger.LogInformation($"File deleted: {absolutePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {absolutePath}");
                return false;
            }
        }

        public Task<string> GetFileUrlAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult(string.Empty);

            if (filePath.StartsWith("http") || filePath.StartsWith("www."))
                return Task.FromResult(filePath);

            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:5001";
            return Task.FromResult($"{baseUrl}/{filePath.TrimStart('/')}");
        }

        public async Task<string> HandleExternalVideoAsync(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
                return string.Empty;

            // YouTube link kezelése
            if (videoUrl.Contains("youtube.com") || videoUrl.Contains("youtu.be"))
            {
                return ConvertToEmbedUrl(videoUrl);
            }

            // Vimeo link kezelése
            if (videoUrl.Contains("vimeo.com"))
            {
                return ConvertToEmbedUrl(videoUrl);
            }

            // Egyéb külső videók
            return videoUrl;
        }

        public Task<IEnumerable<string>> GetAllowedExtensionsAsync(FileType fileType)
        {
            var configKey = "FileUpload:AllowedExtensions";
            var allExtensions = _configuration.GetSection(configKey).Get<string[]>() ?? new[] { ".mp4", ".pdf" };

            var extensionsByType = fileType switch
            {
                FileType.Video => new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv" },
                FileType.Document => new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt" },
                FileType.Image => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" },
                _ => allExtensions
            };

            return Task.FromResult(extensionsByType.AsEnumerable());
        }

        private string GetUploadFolder(FileType fileType, string userId)
        {
            var basePath = _configuration["FileUpload:UploadPath"] ?? "wwwroot/uploads";
            var typeFolder = fileType.ToString().ToLower();
            
            return Path.Combine(_environment.WebRootPath, basePath, typeFolder, userId);
        }

        private string GetRelativePath(string absolutePath)
        {
            var webRootPath = _environment.WebRootPath;
            if (absolutePath.StartsWith(webRootPath))
            {
                return absolutePath.Substring(webRootPath.Length).Replace("\\", "/").TrimStart('/');
            }
            return absolutePath;
        }

        private string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(_environment.WebRootPath, relativePath);
        }

        private string ConvertToEmbedUrl(string videoUrl)
        {
            // YouTube embed URL konverzió
            if (videoUrl.Contains("youtube.com/watch"))
            {
                var videoId = ExtractYouTubeVideoId(videoUrl);
                return $"https://www.youtube.com/embed/{videoId}";
            }
            else if (videoUrl.Contains("youtu.be"))
            {
                var videoId = videoUrl.Split('/').Last();
                return $"https://www.youtube.com/embed/{videoId}";
            }
            // Vimeo embed URL konverzió
            else if (videoUrl.Contains("vimeo.com"))
            {
                var videoId = videoUrl.Split('/').Last();
                return $"https://player.vimeo.com/video/{videoId}";
            }

            return videoUrl;
        }

        private string ExtractYouTubeVideoId(string url)
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["v"] ?? uri.Segments.Last();
        }
    }
}