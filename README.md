# SecureAssist

SecureAssist is an enterprise AI-augmented workflow API designed for a secure coding training program. This project serves as a foundational scaffolding for building secure-by-default applications.

## Project Overview

- **Purpose**: Educational tool for secure coding masterclasses.
- **Goal**: Provide a clean, extensible, and security-aware project foundation.
- **Tech Stack**: .NET 8 ASP.NET Core Web API, EF Core (SQLite), JWT Authentication.

## Architecture

The project follows a **Layered Architecture** with a clear separation of concerns:

- **SecureAssist.API**: Presentation layer, handling HTTP requests, middleware, and documentation (Swagger).
- **SecureAssist.Application**: Business logic, service interfaces, and application services.
- **SecureAssist.Domain**: Core entities, value objects, and domain logic.
- **SecureAssist.Infrastructure**: Data persistence (EF Core), AI service implementations, and external integrations.

## Getting Started

### Prerequisites

- .NET 8 SDK installed.

### Setup and Build

You can use the provided setup scripts in the `/scripts` directory to initialize the solution:

**On Windows (PowerShell):**
```powershell
.\scripts\setup.ps1
```

**On Linux/macOS (Bash):**
```bash
chmod +x ./scripts/setup.sh
./scripts/setup.sh
```

These scripts will:
1. Create the solution and project structure.
2. Add necessary project references.
3. Install required NuGet packages.
4. Restore and build the solution.

## Roadmap & Evolution

This project is designed to evolve module-by-module. Initial scaffolding includes:
- Health check endpoints.
- AuthController & AiController placeholders.
- Base DI wiring for AI Services.
- SQLite configuration via EF Core.
- JWT configuration.

## ⚠️ Warning

**This project is for educational purposes only and is NOT production-ready.** It contains numerous `// TODO` markers for security enhancements that would be required in a real-world enterprise environment. Refer to `SECURITY_NOTES.md` for more details.
