# setup.ps1
# Create Solution
dotnet new sln -n SecureAssist

# Create Projects
dotnet new webapi -n SecureAssist.API -o src/SecureAssist.API --use-controllers
dotnet new classlib -n SecureAssist.Application -o src/SecureAssist.Application
dotnet new classlib -n SecureAssist.Domain -o src/SecureAssist.Domain
dotnet new classlib -n SecureAssist.Infrastructure -o src/SecureAssist.Infrastructure

# Add Projects to Solution
dotnet sln add src/SecureAssist.API/SecureAssist.API.csproj
dotnet sln add src/SecureAssist.Application/SecureAssist.Application.csproj
dotnet sln add src/SecureAssist.Domain/SecureAssist.Domain.csproj
dotnet sln add src/SecureAssist.Infrastructure/SecureAssist.Infrastructure.csproj

# Add Project References
dotnet add src/SecureAssist.Application/SecureAssist.Application.csproj reference src/SecureAssist.Domain/SecureAssist.Domain.csproj
dotnet add src/SecureAssist.Infrastructure/SecureAssist.Infrastructure.csproj reference src/SecureAssist.Application/SecureAssist.Application.csproj
dotnet add src/SecureAssist.Infrastructure/SecureAssist.Infrastructure.csproj reference src/SecureAssist.Domain/SecureAssist.Domain.csproj
dotnet add src/SecureAssist.API/SecureAssist.API.csproj reference src/SecureAssist.Application/SecureAssist.Application.csproj
dotnet add src/SecureAssist.API/SecureAssist.API.csproj reference src/SecureAssist.Infrastructure/SecureAssist.Infrastructure.csproj

# Add NuGet Packages
dotnet add src/SecureAssist.Infrastructure/SecureAssist.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add src/SecureAssist.API/SecureAssist.API.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add src/SecureAssist.API/SecureAssist.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/SecureAssist.API/SecureAssist.API.csproj package Swashbuckle.AspNetCore

# Restore and Build
dotnet restore
dotnet build
