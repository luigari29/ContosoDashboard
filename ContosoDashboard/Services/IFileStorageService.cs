namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string relativeFolderPath);
    Task DeleteAsync(string relativeFilePath);
    Task<Stream> DownloadAsync(string relativeFilePath);
}
