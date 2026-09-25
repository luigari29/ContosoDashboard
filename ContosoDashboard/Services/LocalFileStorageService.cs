namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService()
    {
        _rootPath = Path.Combine(Directory.GetCurrentDirectory(), "AppData", "uploads");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, string relativeFolderPath)
    {
        var sanitizedFolder = (relativeFolderPath ?? string.Empty)
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar)
            .Trim();

        var targetDirectory = Path.Combine(_rootPath, sanitizedFolder);
        Directory.CreateDirectory(targetDirectory);

        var targetPath = Path.Combine(targetDirectory, fileName);

        await using var output = File.Create(targetPath);
        await fileStream.CopyToAsync(output);

        var relativePath = Path.Combine(relativeFolderPath, fileName)
            .Replace('\\', '/');

        return relativePath.TrimStart('/');
    }

    public Task DeleteAsync(string relativeFilePath)
    {
        var normalizedPath = NormalizeRelativePath(relativeFilePath);
        if (string.IsNullOrWhiteSpace(normalizedPath))
        {
            return Task.CompletedTask;
        }

        var fullPath = Path.Combine(_rootPath, normalizedPath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public async Task<Stream> DownloadAsync(string relativeFilePath)
    {
        var normalizedPath = NormalizeRelativePath(relativeFilePath);
        var fullPath = Path.Combine(_rootPath, normalizedPath.Replace('/', Path.DirectorySeparatorChar));

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Document file not found.", fullPath);
        }

        return await Task.FromResult<Stream>(File.OpenRead(fullPath));
    }

    private static string NormalizeRelativePath(string relativeFilePath)
    {
        return (relativeFilePath ?? string.Empty)
            .Trim()
            .TrimStart('/');
    }
}
