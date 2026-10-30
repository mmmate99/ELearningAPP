namespace ElearningAPI.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string userId, FileType fileType);
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> GetFileUrlAsync(string filePath);
        Task<string> HandleExternalVideoAsync(string videoUrl);
        Task<IEnumerable<string>> GetAllowedExtensionsAsync(FileType fileType);
    }

    public enum FileType
    {
        Video,
        Document,
        Image,
        Other
    }
}