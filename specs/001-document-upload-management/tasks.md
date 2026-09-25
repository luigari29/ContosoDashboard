# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: No dedicated test tasks were added because the feature specification did not explicitly request them, and the task generation rules treat tests as optional unless they are explicitly requested or TDD is requested.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the repository for the document feature and establish the storage/security conventions used across all stories.

- [X] T001 Create document feature directories and safe storage layout under `ContosoDashboard/AppData/uploads/` and `ContosoDashboard/Services/`
- [X] T002 [P] Add document metadata and audit model scaffolding in `ContosoDashboard/Models/Document.cs`, `ContosoDashboard/Models/DocumentShare.cs`, and `ContosoDashboard/Models/DocumentActivity.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story implementation begins.

- [X] T003 Define storage abstraction and upload contract in `ContosoDashboard/Services/IFileStorageService.cs`
- [X] T004 [P] Implement the local storage provider in `ContosoDashboard/Services/LocalFileStorageService.cs`
- [X] T005 [P] Register document services and storage dependency injection in `ContosoDashboard/Program.cs`
- [X] T006 Add document DbSets and relationship configuration in `ContosoDashboard/Data/ApplicationDbContext.cs`
- [X] T007 Create the document service contract in `ContosoDashboard/Services/IDocumentService.cs`
- [X] T008 Add file validation, access checks, and safe path generation in `ContosoDashboard/Services/DocumentService.cs`
- [X] T009 Create the async scan queue publisher in `ContosoDashboard/Services/DocumentScanQueueService.cs` for Azure Functions / Queue Storage integration

**Checkpoint**: Foundation ready - user story implementation can begin in parallel.

---

## Phase 3: User Story 1 - Upload and organize personal and project documents (Priority: P1) 🎯 MVP

**Goal**: Let users upload valid documents with metadata, validate them securely, and retrieve their own files through a browsing and search experience.

**Independent Test**: An authenticated employee can upload a valid file, see it in their document list, and locate it via search or filter without exposing unrelated files.

### Implementation for User Story 1

- [X] T010 [P] [US1] Implement Document entity fields and validation rules in `ContosoDashboard/Models/Document.cs`
- [X] T011 [P] [US1] Implement upload, metadata persistence, and file save workflow in `ContosoDashboard/Services/DocumentService.cs`
- [X] T012 [US1] Implement the queued virus-scan workflow and status update logic in `ContosoDashboard/Services/DocumentScanQueueService.cs`
- [X] T013 [US1] Add upload UI and validation form in `ContosoDashboard/Pages/Documents.razor` and `ContosoDashboard/Pages/Documents.razor.cs`
- [X] T014 [US1] Add personal document list, sorting, filtering, and search in `ContosoDashboard/Pages/Documents.razor`
- [X] T015 [US1] Add success/error messaging and rejected upload handling in `ContosoDashboard/Pages/Documents.razor`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently.

---

## Phase 4: User Story 2 - Work with project documents and share them with team members (Priority: P1)

**Goal**: Allow users to view project documents, share files with specific recipients, and enforce access controls for shared and project-scoped content.

**Independent Test**: A team member can open a project, review project documents, and share a document with another authorized user who then sees it in the shared documents view.

### Implementation for User Story 2

- [ ] T016 [P] [US2] Extend `ContosoDashboard/Pages/ProjectDetails.razor` to display project documents and project-level upload actions
- [ ] T017 [US2] Implement access rules for project documents and share recipients in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T018 [US2] Add shared-with-me listing and recipient notification flow in `ContosoDashboard/Pages/Documents.razor` and `ContosoDashboard/Services/NotificationService.cs`
- [ ] T019 [US2] Implement download, preview, delete, and replace-file actions in `ContosoDashboard/Pages/Documents.razor`
- [ ] T020 [US2] Add document share entity and permission checks in `ContosoDashboard/Models/DocumentShare.cs` and `ContosoDashboard/Services/DocumentService.cs`

**Checkpoint**: At this point, User Stories 1 and 2 should both work independently and provide core document collaboration value.

---

## Phase 5: User Story 3 - Maintain and review document records for governance and compliance (Priority: P2)

**Goal**: Provide administrators with auditable document activity tracking and reporting without exposing unrelated user data.

**Independent Test**: An administrator can review document activity and generate report data for upload volume, active uploaders, and document type patterns.

### Implementation for User Story 3

- [ ] T021 [P] [US3] Implement activity logging for uploads, downloads, deletes, replacements, and shares in `ContosoDashboard/Models/DocumentActivity.cs` and `ContosoDashboard/Services/DocumentService.cs`
- [ ] T022 [US3] Create reporting logic in `ContosoDashboard/Services/DocumentReportingService.cs`
- [ ] T023 [US3] Add admin document reporting page in `ContosoDashboard/Pages/AdminDocuments.razor` or the nearest existing admin/reporting page
- [ ] T024 [US3] Enforce administrator-only access rules and summarize report filters in `ContosoDashboard/Services/DocumentReportingService.cs`

**Checkpoint**: The governance and audit story should be independently functional for admin review.

---

## Phase 6: User Story 4 - Attach and manage documents from task and dashboard workflows (Priority: P2)

**Goal**: Surface document actions in the user’s existing daily workflow so document work feels integrated into task and dashboard usage rather than separate from it.

**Independent Test**: A user can attach a file from the task page and see recent activity on the dashboard without leaving the main workflow.

### Implementation for User Story 4

- [ ] T025 [P] [US4] Add document attachment controls in `ContosoDashboard/Pages/Tasks.razor` and related task components
- [ ] T026 [US4] Associate uploaded documents with the correct project and task context in `ContosoDashboard/Services/DocumentService.cs`
- [ ] T027 [US4] Extend dashboard summary and recent document widget in `ContosoDashboard/Pages/Index.razor` and `ContosoDashboard/Services/DashboardService.cs`
- [ ] T028 [US4] Add recent document count and latest upload data to dashboard summary components in `ContosoDashboard/Pages/Index.razor`

**Checkpoint**: All major document user stories are now independently functional.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improve the quality and consistency of the cross-story implementation before final validation.

- [ ] T029 [P] Update the user-facing documentation and quickstart steps in `README.md` and `specs/001-document-upload-management/quickstart.md`
- [ ] T030 [P] Review and harden authorization, file validation, and lifecycle operations across `ContosoDashboard/Services/` and `ContosoDashboard/Pages/`
- [ ] T031 Validate end-to-end scenarios from the feature quickstart and confirm secure handling of rejected files and queue updates

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational completion
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational - no dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational and may integrate with US1, but should remain independently testable
- **User Story 3 (P2)**: Can start after Foundational and may integrate with US1/US2, but should remain independently testable
- **User Story 4 (P2)**: Can start after Foundational and may integrate with US1/US2, but should remain independently testable

### Within Each User Story

- Core data and validation before UI
- Service layer before page integration
- Story complete before moving to the next priority

### Parallel Opportunities

- All tasks in Setup marked [P] can run in parallel
- All tasks in Foundational marked [P] can run in parallel within Phase 2
- Once the Foundation is complete, User Story 1 and User Story 2 can be implemented in parallel by separate contributors
- Within a story, the model and service tasks marked [P] can be worked concurrently
- Tasks in the Polish phase can run in parallel once all stories are complete

---

## Parallel Example: User Story 1

```bash
# Model and queue preparation
Task: "Implement Document entity fields and validation rules in ContosoDashboard/Models/Document.cs"
Task: "Implement upload, metadata persistence, and file save workflow in ContosoDashboard/Services/DocumentService.cs"

# UI work in parallel after service contract is ready
Task: "Add upload UI and validation form in ContosoDashboard/Pages/Documents.razor"
Task: "Add personal document list, sorting, filtering, and search in ContosoDashboard/Pages/Documents.razor"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate the story independently
5. Stop and review before moving to broader collaboration features

### Incremental Delivery

1. Setup + Foundational -> foundation ready
2. User Story 1 -> upload and browsing MVP
3. User Story 2 -> project and shared document collaboration
4. User Story 3 -> governance and audit reporting
5. User Story 4 -> dashboard and task workflow integration
6. Polish -> final security, performance, and docs review

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Developer A: User Story 1
3. Developer B: User Story 2
4. Developer C: User Story 3
5. Developer D: User Story 4
6. All complete the final polish and validation together
