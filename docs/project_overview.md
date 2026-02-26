# SecureAssist - Project Overview
**Generated Date:** 2026-02-26 11:20:00 (UTC)

## Project Intent
SecureAssist is a .NET 8 ASP.NET Core application designed as a **Security Training Ground**. 

The primary objective is to provide a realistic, enterprise-grade layered architecture that is **intentionally vulnerable** to common input validation and trust-boundary flaws. This allows developers and security researchers to practice structured hardening, implementing validation logic, and enforcing multi-tenant isolation.

> [!IMPORTANT]
> **This project contains intentional security flaws.**
> Current features allow bypasses of ownership checks, tenant isolation, and size restrictions. These are "vulnerabilities by design" for educational purposes.

---

## High-Level Architecture
The project follows a Clean/Layered Architecture approach to mimic real-world enterprise systems.

### 1. SecureAssist.API (Presentation Layer)
- **Role:** Entry point for HTTP requests.
- **Responsibility:** Model binding, routing, and preliminary authentication (JWT).
- **Current State:** Controllers are thin wrappers around MediatR, passing requests blindly to the application layer without validation.

### 2. SecureAssist.Application (Core Logic)
- **Role:** Orchestrates business flows.
- **Technology:** Uses **MediatR** for Command/Query separation.
- **Key Folders:** `Features/{ModuleName}/Commands`
- **Current State:** Handlers implement "happy path" logic only. They perform direct file operations and database persistence without checking permissions or data integrity.

### 3. SecureAssist.Domain (Heart of the System)
- **Role:** Contains entities, value objects, and domain-level interfaces.
- **Entities:** `Document`, `AIInteraction`, `SearchLog`, `WorkflowActionRecord`.
- **Current State:** Entities lack guardrails or complex business rules.

### 4. SecureAssist.Infrastructure (Data & External Services)
- **Role:** Implementation of database access (EF Core) and external service integrations (Mock AI).
- **Database:** SQLite (In-Memory/Local file) for rapid prototyping and easy training setup.

---

## Key Functionality & Request Flows

### AI Interaction (`/api/ai/ask`)
- **Flow:** `AiController` -> `AskAiCommand` -> `AskAiCommandHandler` -> `AppDbContext`
- **Vulnerability Note:** Accepts a `metadata` dictionary and `systemOverride` string without sanitization.

### Document Management (`/api/documents`)
- **Upload Flow:** `DocumentsController` -> `UploadDocumentCommand` -> `File system write` + `AppDbContext`
- **Update Flow:** `DocumentsController` -> `UpdateDocumentCommand` -> `AppDbContext (Blind Update)`
- **Vulnerability Note:** `DisableRequestSizeLimit` is applied to uploads. `UpdateDocument` allows a user to "take over" a document by changing `OwnerId` or `TenantId` in the body.

### Search Engine (`/api/search`)
- **Flow:** `SearchController` -> `PerformSearchCommand` -> `SearchLog` + `Document Query`
- **Vulnerability Note:** Query parameters for paging and sorting are trustingly passed into EF logic.

---

## Technical Stack
- **Framework:** .NET 8.0
- **Patterns:** Mediator, CQRS (Lite), Layered Architecture
- **Libraries:**
  - `MediatR` for decoupled messaging.
  - `Entity Framework Core` for persistence.
  - `Swashbuckle` for Swagger documentation.

## How to use this for Training
1. **Identify Trust Boundaries:** Look at where data enters the system in Controllers and Handlers.
2. **Implement Validation:** Use FluentValidation or DataAnnotations to fix the "Under-Validated" state.
3. **Enforce Tenant Isolation:** Modify the `AppDbContext` or use MediatR decorators to ensure one tenant cannot access another's data.
4. **Sanitize Inputs:** Specifically focus on the `metadata` and `systemOverride` fields in the AI module.
