# Research: Document Upload and Management

## Decision: Use a local file storage abstraction with metadata persisted in EF Core

The feature will store uploaded binary content in a secure local directory outside `wwwroot` and persist metadata in the existing SQL Server LocalDB context. This matches the offline-first training requirement and keeps the design ready for a future Azure Blob implementation through an `IFileStorageService` abstraction.

### Rationale

- The repository already uses layered architecture with `Data`, `Models`, and `Services` and does not currently include a storage abstraction.
- The business requirement explicitly calls for offline storage and future cloud migration support without rewriting the app.
- Saving user-controlled file names safely requires GUID-based filenames and directory separation from the web root to reduce path traversal and direct download risks.
- The app’s authentication model is already local and role-based, so the document feature should follow the same access-control pattern with service-layer enforcement.

### Alternatives considered

- Store files directly in `wwwroot`: rejected because it exposes file content to unauthenticated access patterns and conflicts with the security requirement.
- Use direct database storage for documents: rejected because it complicates large-file handling, storage lifecycle, and future migration.
- Introduce a separate backend API: rejected because the project is a single Blazor Server application and the requirement explicitly says no major rewrite.

---

## Decision: Enforce access control and ownership in services, not only at the UI layer

The product requirement for role-based permissions and IDOR protection will be enforced in service methods and page authorization checks. Users will not be trusted to decide what is visible based only on the UI state.

### Rationale

- The app already enforces authorization checks at service and page layers for notifications and tasks.
- The requirements explicitly call out IDOR protection, project-level enforcement, and user isolation.
- This reduces the risk of bypass through URL manipulation or direct method invocation.

### Alternatives considered

- UI-only filtering: rejected because it can be bypassed and does not protect the underlying data access.
- Global allow-all project access: rejected because it violates least privilege and the user story definitions.

---

## Decision: Model document metadata as text-based categories and integer-based document IDs

The feature will use integer `DocumentId` values and text-based category values such as `Project Documents` and `Personal Files`, in line with the technical constraints already captured in the stakeholder document.

### Rationale

- The repo’s current data model uses integer keys across user, project, and task entities.
- Text categories are easier to maintain in a training app and allow straightforward UI filtering without enum migration complexity.
- The requirement specifically calls for `FileType` to support long MIME strings and `FilePath` to support GUID-based names.

### Alternatives considered

- Int enum categories: rejected because it adds unnecessary mapping and migration work without providing business value.
- GUID document IDs: rejected because it breaks the repository’s existing integer-key consistency requirement.

---

## Decision: Integrate document workflows into the existing task and dashboard UI patterns

The project will adopt the current Blazor Server page and service conventions rather than creating a separate micro-app or parallel dashboard module. Tasks and project pages will expose document attach/upload actions, and the dashboard will show recent document activity and counts.

### Rationale

- The current app already centralizes dashboard, project, and task views in pages and services.
- This supports the “3 clicks or fewer” usability goal without requiring a complete redesign.
- The architecture remains consistent with the project’s educational and minimal-change objectives.

### Alternatives considered

- Create a standalone document portal: rejected because it breaks flow and creates duplicate user patterns.
- Add document features only via admin pages: rejected because it does not support daily user workflows and task-based collaboration.

---

## Decision: Treat document sharing as a first-class relationship, not a hidden UI effect

Sharing will be represented as discrete records with a `DocumentShare` entity and notification generation when a user receives a new shared document.

### Rationale

- This supports auditability, permission checks, and report generation requirements.
- It keeps sharing explicit and reviewable instead of relying on ad hoc metadata or folder assignments.
- It creates the cleanest path to future reporting and security review.

### Alternatives considered

- Only use project membership to grant access: rejected because it does not satisfy the explicit “share with specific users or teams” requirement.
- Store shares as free-form tags or strings: rejected because it is not queryable or auditable.

---

## Decision: Add an asynchronous virus-scan job using Azure Functions and Queue Storage triggers

After a document upload is validated and stored, the app will enqueue a scan request for asynchronous malware processing. In the Azure-ready design, an Azure Function with a Queue Storage trigger will read the message, invoke the scan workflow, and update the document record to `Queued`, `Scanning`, `Clean`, or `Rejected` based on the results. In the offline training environment, the app will use a lightweight local stub or no-op scanner so the workflow remains operational without Azure services.

### Rationale

- The security requirement explicitly requires uploaded files to be scanned before they are considered safe for access.
- The user upload flow should not block the UI for long antivirus checks, especially for large or network-slow uploads.
- Azure Functions + Queue Storage is a natural fit for a decoupled, scalable background task and matches the repository’s migration-ready design.
- The queue pattern preserves the current app architecture without forcing a major rewrite while still supporting eventual cloud deployment.

### Alternatives considered

- Perform malware scanning synchronously in the web request: rejected because it makes upload completion slower and couples UI responsiveness to scanning time.
- Skip a background queue and use a direct in-process scan: rejected because it does not prepare the app for cloud-scale or asynchronous retry behavior.
- Use a separate app or service for every scan operation: rejected because it increases deployment complexity beyond the current training scope.

### Azure design flow

1. Upload completes and metadata is written to the database with a `ScanStatus` of `Queued`.
2. The application publishes a message to a document scan queue containing document ID, storage path, and user context.
3. An Azure Function trigger listens to the queue and begins the scan workflow.
4. The scanner validates the file, marks the item as `Clean` or `Rejected`, and updates the document record.
5. If the file is rejected, the storage service removes the file and flags the document as inaccessible until an authorized user reattempts or the item is cleaned up.
6. Notifications or audit records are emitted whenever the scan result changes state.

This pattern keeps the upload path fast while preserving a migration path to Azure-native security processing.
