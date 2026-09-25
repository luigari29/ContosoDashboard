using Microsoft.EntityFrameworkCore;
using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IDocumentService
{
    Task<List<Document>> GetUserDocumentsAsync(int userId, string? searchText = null, string? category = null);
    Task<Document> UploadAsync(int userId, string title, string? description, string category, int? projectId, Stream fileStream, string fileName, string contentType, string? tags);
}

public class DocumentService : IDocumentService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".ppt",
        ".pptx",
        ".txt",
        ".jpg",
        ".jpeg",
        ".png"
    };

    private static readonly HashSet<string> AllowedCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "Project Documents",
        "Team Resources",
        "Personal Files",
        "Reports",
        "Presentations",
        "Other"
    };

    private readonly ApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;

    public DocumentService(ApplicationDbContext context, IFileStorageService fileStorageService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
    }

    public async Task<List<Document>> GetUserDocumentsAsync(int userId, string? searchText = null, string? category = null)
    {
        var query = _context.Documents
            .AsNoTracking()
            .Where(d => d.UploadedByUserId == userId && !d.IsDeleted)
            .Include(d => d.Project)
            .Include(d => d.UploadedByUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(d => d.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var normalized = searchText.Trim();
            query = query.Where(d =>
                d.Title.Contains(normalized) ||
                (d.Description != null && d.Description.Contains(normalized)) ||
                (d.Tags != null && d.Tags.Contains(normalized)) ||
                d.FileName.Contains(normalized) ||
                (d.Project != null && d.Project.Name.Contains(normalized)) ||
                d.UploadedByUser.DisplayName.Contains(normalized));
        }

        return await query
            .OrderByDescending(d => d.UploadedDate)
            .ToListAsync();
    }

    public async Task<Document> UploadAsync(int userId, string title, string? description, string category, int? projectId, Stream fileStream, string fileName, string contentType, string? tags)
    {
        if (fileStream == null)
        {
            throw new InvalidOperationException("A file must be provided for upload.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Document title is required.");
        }

        if (string.IsNullOrWhiteSpace(category) || !AllowedCategories.Contains(category.Trim()))
        {
            throw new InvalidOperationException("Document category is required and must be selected from the supported list.");
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("This file type is not supported.");
        }

        if (fileStream.Length > 25 * 1024 * 1024)
        {
            throw new InvalidOperationException("Files larger than 25 MB are not allowed.");
        }

        var hasProjectAccess = !projectId.HasValue || await _context.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == projectId.Value && pm.UserId == userId);

        if (!hasProjectAccess)
        {
            throw new UnauthorizedAccessException("You do not have access to upload documents for this project.");
        }

        var safeFileName = $"{Guid.NewGuid():N}{extension}";
        var folder = projectId.HasValue ? $"users/{userId}/projects/{projectId.Value}" : $"users/{userId}/personal";

        var relativePath = await _fileStorageService.UploadAsync(fileStream, safeFileName, contentType, folder);

        var document = new Document
        {
            Title = title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Category = category.Trim(),
            ProjectId = projectId,
            UploadedByUserId = userId,
            FileName = fileName,
            FilePath = relativePath,
            FileType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            FileSizeBytes = fileStream.Length,
            UploadedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            Tags = string.IsNullOrWhiteSpace(tags) ? null : tags.Trim(),
            IsDeleted = false
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return await _context.Documents
            .Include(d => d.Project)
            .Include(d => d.UploadedByUser)
            .FirstAsync(d => d.DocumentId == document.DocumentId);
    }
}
