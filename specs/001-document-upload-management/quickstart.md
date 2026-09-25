# Quickstart: Document Upload and Management Validation

## Prerequisites

- .NET 8 SDK installed
- SQL Server LocalDB available on the local machine
- The application repo checked out on the `001-document-upload-management` branch
- Local environment configured to run the Blazor app in Development mode

## Setup

1. Open a terminal in the repo root.
2. Restore dependencies:
   `dotnet restore`
3. Run the application:
   `dotnet run --project ContosoDashboard/ContosoDashboard.csproj`
4. Sign in with one of the mock users defined in the app, such as `ni.kang@contoso.com`.

## Validation Scenarios

### 1. Upload a valid document

- Navigate to the documents area or the project/task page.
- Select a supported file such as a PDF or Office document under 25 MB.
- Enter a title and select a category.
- Submit the upload.

Expected outcome:

- The upload succeeds.
- A confirmation message appears.
- The document appears in the user’s documents list with title, category, file size, and timestamp.

### 2. Reject invalid file content

- Try to upload a file with an unsupported extension or a file larger than 25 MB.

Expected outcome:

- The upload is blocked.
- The UI shows a clear validation message.
- No record is persisted for the rejected upload.

### 3. Verify project access controls

- Log in as a team member with access to a project.
- Open the project document list.
- Attempt to access a document outside the project or not shared with the user.

Expected outcome:

- Authorized users can view and download project documents.
- Unauthorized users see an access denial or no document row.

### 4. Share a document with another user

- Upload a document.
- Share it with a second user who has a valid relationship to the workspace.
- Confirm the recipient receives an in-app notification.

Expected outcome:

- The recipient sees the document in the shared-with-me area.
- The notification is created and marked unread until opened.

### 5. Confirm dashboard integration

- Upload a document and then return to the dashboard home page.

Expected outcome:

- The recent documents widget displays the latest uploaded item.
- The summary card reflects the document count or recent activity for the current user.

## Exit Criteria

The feature is considered ready for implementation review when all of the validation scenarios above show the expected behavior and the build passes with no blocking errors.
