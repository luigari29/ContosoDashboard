# Document Service Contract

## Purpose

This contract defines the expected service-level interface for document upload, metadata handling, access control, and storage abstraction.

## Storage Abstraction

```csharp
public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string safeFileName, string contentType, string relativeFolderPath);
    Task DeleteAsync(string relativeFilePath);
    Task<Stream> DownloadAsync(string relativeFilePath);
    Task<string> GetUrlAsync(string relativeFilePath, TimeSpan expiration);
}
```

### Contract behavior

- `UploadAsync` stores file content outside `wwwroot` and returns a safe relative path or blob key.
- `DeleteAsync` removes the stored file and does not leave orphaned data behind.
- `DownloadAsync` retrieves the stored file only after authorization checks pass in the service layer.
- `GetUrlAsync` is intended for future Azure Blob migration, but the local training version may return the local file path or an internal static route.

## Document Service Interface

```csharp
public interface IDocumentService
{
    Task<Document> UploadAsync(int userId, int? projectId, int? taskId, string title, string? description, string category, Stream fileStream, string fileName, string contentType, List<string>? tags);
    Task<List<Document>> GetUserDocumentsAsync(int userId, DocumentQuery query);
    Task<List<Document>> GetProjectDocumentsAsync(int userId, int projectId);
    Task<Document?> GetByIdAsync(int documentId, int userId);
    Task<bool> UpdateMetadataAsync(int documentId, int requestingUserId, UpdateDocumentRequest request);
    Task<bool> ReplaceFileAsync(int documentId, int requestingUserId, Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteAsync(int documentId, int requestingUserId);
    Task<List<Document>> GetSharedDocumentsAsync(int userId);
    Task<bool> ShareAsync(int documentId, int ownerUserId, int sharedWithUserId);
}
```

## Validation Rules

- Supported file extensions must be validated before upload.
- File size must not exceed 25 MB.
- Title and category are required.
- Unauthorized users must not see or download documents outside their permitted access scope.
- The service must create a document activity record for audit events.

## Error Behavior

- `UploadAsync` rejects unsupported file types with a validation error.
- `DeleteAsync` rejects unauthorized requests with an authorization failure.
- `ShareAsync` rejects invalid or inactive user relationships and records the failure in the activity log.
- File storage failures must not persist a metadata record without a corresponding successful write.

## Example Request

```json
{
  "title": "Project plan v2",
  "description": "Updated planning draft",
  "category": "Project Documents",
  "projectId": 1,
  "taskId": null,
  "tags": ["planning", "draft"]
}
```

## Example Response

```json
{
  "documentId": 42,
  "title": "Project plan v2",
  "category": "Project Documents",
  "fileType": "application/pdf",
  "fileSizeBytes": 1843200,
  "uploadedByUserId": 4,
  "uploadedDate": "2026-09-25T12:00:00Z"
}
```
