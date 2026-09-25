# Data Model: Document Upload and Management

## Core Entities

### Document

Represents a file uploaded to the system and the metadata attached to it.

- DocumentId: integer, primary key
- Title: required string, human-readable document name
- Description: optional string, max length aligned to the product requirements
- Category: required string value from the approved category list; stored as text, not enum
- ProjectId: optional integer reference to a project if the document belongs to a project
- TaskId: optional integer reference to a task if the document is attached to a task
- UploadedByUserId: required integer reference to the user who uploaded the file
- FileName: generated safe file name, not user-supplied
- FilePath: relative or storage-local file path, safe for local and future cloud storage abstraction
- FileType: required string, supports MIME values up to 255 characters
- FileSizeBytes: integer, must be validated against 25 MB limit
- UploadedDate: required UTC timestamp
- UpdatedDate: required UTC timestamp
- IsDeleted: boolean flag for soft-delete or cleanup state, if used by the implementation
- Tags: optional text field or separate join model for search support

Relationships:

- Many documents belong to one uploader (`User`)
- Many documents may belong to one project (`Project`), optional
- Many documents may belong to one task (`TaskItem`), optional
- One document can have many share records (`DocumentShare`)
- One document can have many activity records (`DocumentActivity`)

Validation rules:

- Title is required and non-empty.
- Category must be one of: `Project Documents`, `Team Resources`, `Personal Files`, `Reports`, `Presentations`, `Other`.
- FileSizeBytes must be > 0 and <= 25 MB.
- FileType must be recognized and be on the allowlist.
- FilePath must never be user-supplied; it must be generated from a GUID-safe path.
- ProjectId is allowed to be null when the document is personal or not tied to a project.

### DocumentShare

Represents explicit access granted beyond the base project membership rights.

- DocumentShareId: integer, primary key
- DocumentId: required integer
- SharedWithUserId: required integer
- SharedByUserId: required integer
- SharedDate: required UTC timestamp
- IsActive: boolean, indicates whether the share remains in effect

Relationships:

- Many share records belong to one document
- Many share records are granted to one user
- One share record is created by one user

Validation rules:

- A user cannot share a document with themselves unless the product explicitly allows it; default should be disallowed.
- Share records must be unique per document and recipient when active.
- A share record is invalid if the document or user does not exist.

### DocumentActivity

Captures audit data for reporting and compliance.

- ActivityId: integer, primary key
- DocumentId: required integer
- UserId: required integer
- ActivityType: required string such as `Upload`, `Download`, `Delete`, `Share`, `Replace`
- ActivityDate: required UTC timestamp
- Details: optional text, includes a summary of the operation

Relationships:

- Many activity records belong to one document
- Many activity records are recorded by one user

Validation rules:

- ActivityType must be one of the recognized document actions.
- UserId must refer to an authenticated, existing user.
- ActivityDate must be set automatically by the service layer.

## Existing Repository Entities Reused

### User

The existing `User` model is reused for ownership, role checks, and sharing recipients.

- Role values remain `Employee`, `TeamLead`, `ProjectManager`, and `Administrator`.
- Access checks are still based on current mock auth claims and project membership rules.

### Project

The existing `Project` entity remains the primary grouping for project documents.

- Project membership remains the baseline access check for team documents.
- `ProjectManagerId` is the owner for project document governance and project-level access review.

### TaskItem

The existing `TaskItem` entity supports attachment and task-related document context.

- Optional association to `ProjectId` allows documents to inherit project context from the task.
- Task pages can attach or display related documents without separate domain models.

## Relationship Summary

```text
User 1 --- * Document
Project 1 --- * Document
TaskItem 1 --- * Document
Document 1 --- * DocumentShare
Document 1 --- * DocumentActivity
```

## Implementation Notes

- Use integer IDs to preserve consistency with the existing `User`, `Project`, and `TaskItem` model conventions.
- Store category text values instead of enum-backed integer IDs to keep the UI and filtering logic straightforward.
- Use GUID-based generated file names so file paths remain unique and safe.
- Store all file content outside the web root and behind authorization checks.
- Ensure activity records are created in the service layer whenever document lifecycle actions occur.
