<!--
Sync Impact Report
- Version change: 0.0.0 -> 1.0.0
- Modified principles: N/A -> I. Mission-First Delivery, II. Security by Default, III. Evidence-Driven Delivery, IV. Architecture for Change, V. Simplicity & Clarity
- Added sections: Product & Training Guardrails, Delivery Workflow & Review
- Removed sections: none
- Deferred TODOs: TODO(RATIFICATION_DATE): original constitution adoption date was not recorded in the repository; confirm before formal ratification.
-->

# ContosoDashboard Constitution

## Core Principles

### I. Mission-First Delivery
ContosoDashboard exists to teach secure, maintainable, spec-driven software development in a realistic but intentionally limited training environment. Every feature, fix, and refactor must support the application's stated purpose: project tracking, team coordination, and secure dashboard workflows without adding scope that is unrelated to the training scenario.

This principle exists to prevent scope drift, unnecessary production claims, and features that obscure the educational objective. If a change does not improve the dashboard's training value or business clarity, it is out of scope.

### II. Security by Default
All protected pages, services, and data access paths must enforce authorization and user isolation. The mock authentication model is valid only for training, and every user-facing workflow must preserve least privilege, prevent IDOR patterns, and validate business rules before data is returned or modified.

Security is a non-negotiable product requirement even in a training app. The repository must not introduce unrestricted access, hidden trust assumptions, or insecure patterns that would be unacceptable in a production system.

### III. Evidence-Driven Delivery
Changes must be grounded in clear requirements, explicit acceptance criteria, and verifiable evidence. The team is expected to validate the build and the relevant behavior before merging a change, and no feature is considered complete without a concrete check that it works in context.

This principle keeps the repository honest. Good engineering practice is not assumed; it is demonstrated through tests, compile checks, runtime validation, and review evidence.

### IV. Architecture for Change
The project must maintain clear separation between models, data access, services, and UI concerns. Business logic belongs in services and validated flows, not in presentation layers, and infrastructure choices must remain replaceable without broad, uncontrolled rewrites.

This principle protects maintainability and supports the offline-first architecture described in the project documentation. Abstractions are expected to be intentional, explicit, and easy to reason about.

### V. Simplicity & Clarity
The default design choice is the simplest solution that satisfies the requirement, the security model, and the user workflow. Naming, structure, and documentation must be clear enough for a learner to follow without hidden conventions or unnecessary complexity.

Complexity must be justified by a real need. When a design becomes hard to explain, it is a signal to simplify, split the work, or document the trade-off before continuing.

## Product & Training Guardrails

ContosoDashboard is a training application and must remain aligned with that role. It is not a production platform, and it must not be presented as a secure, supported, or cloud-ready deployment without additional hardening and governance.

- Training scope: features and examples must teach the intended software engineering concepts without introducing speculative product complexity.
- Offline-first model: local development and local data access remain the default, with migration paths documented rather than assumed.
- Mock authentication: sample users, cookie-based sessions, and role checks are for instructional use only and must not be mistaken for a production identity implementation.
- User isolation: data access and page behavior must respect the authenticated user, department boundaries, and service-level authorization checks.
- Documentation responsibility: any new feature or change must remain consistent with the repository's architecture and security documentation.

## Delivery Workflow & Review

Every change in this repository must follow a disciplined workflow that keeps the codebase understandable and reliable.

- Requirements and acceptance criteria must be clear before implementation begins.
- Changes must be reviewed for security, scope alignment, user impact, and maintainability before merge.
- The implementation must be validated with the smallest relevant build or test cycle, and evidence must be captured in the change record or review discussion.
- Large or risky changes must be broken into smaller, reviewable increments instead of hidden, broad refactors.
- Documentation, naming, and architecture choices must support future learners and maintainers rather than only the author.

## Governance

This Constitution governs all repository work. When these rules conflict with a local preference, team habit, or convenience, the Constitution wins. The repository must remain aligned with the stated educational purpose, security standards, and architecture rules described here.

Amendments require a written rationale, a version bump that follows the policy below, and a review that confirms the change is consistent with the repository's purpose. Changes may not be hidden inside unrelated refactors or merged without explicit review evidence.

Versioning policy:
- MAJOR: backward-incompatible governance changes or removal of a core principle
- MINOR: new principle or section added, or a material expansion of current guidance
- PATCH: clarifying wording, terminology cleanup, or non-semantic documentation improvements

Compliance review expectation:
- Reviewers must verify alignment with this Constitution before approving repository changes.
- Any exception or deviation requires documented rationale, clear owner accountability, and a plan to restore compliance.
- The repository's current state must be consistent with the Constitution at all times, not only after a future cleanup.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original constitution adoption date was not recorded in the repository and must be confirmed before formal ratification. | **Last Amended**: 2026-09-25
