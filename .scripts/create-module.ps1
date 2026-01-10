<#
.SYNOPSIS
    Creates a new module with Clean Architecture structure

.DESCRIPTION
    This script creates a new module following the Modular Monolith architecture pattern.
    Each module contains Domain, Application, Infrastructure, and Api layers.

.PARAMETER ModuleName
    The name of the module to create (e.g., "Catalog", "Sales")

.PARAMETER BasePath
    The base path where modules are created (default: "src/Modules")

.EXAMPLE
    .\create-module.ps1 -ModuleName "Catalog"
    Creates a new Catalog module with all layers

.EXAMPLE
    .\create-module.ps1 -ModuleName "Sales" -BasePath "src/Modules"
    Creates a new Sales module at the specified path
#>

param(
    [Parameter(Mandatory=$true, HelpMessage="Enter the module name (e.g., Catalog, Sales)")]
    [ValidateNotNullOrEmpty()]
    [string]$ModuleName,

    [Parameter(Mandatory=$false)]
    [string]$BasePath = "src/Modules"
)

# Color output functions
function Write-Success { param($Message) Write-Host "✓ $Message" -ForegroundColor Green }
function Write-Info { param($Message) Write-Host "ℹ $Message" -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host "⚠ $Message" -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host "✗ $Message" -ForegroundColor Red }

# Validate module name
if ($ModuleName -notmatch '^[A-Z][a-zA-Z0-9]*$') {
    Write-Error "Module name must start with uppercase letter and contain only alphanumeric characters"
    exit 1
}

Write-Info "Creating module: $ModuleName"
Write-Info "Base path: $BasePath"

# Create module directory structure
$modulePath = Join-Path $BasePath $ModuleName
$layers = @("Domain", "Application", "Infrastructure", "Api")

foreach ($layer in $layers) {
    $layerPath = Join-Path $modulePath "$ModuleName.$layer"
    
    if (Test-Path $layerPath) {
        Write-Warning "Layer $layer already exists at $layerPath"
        continue
    }
    
    Write-Info "Creating $ModuleName.$layer..."
    
    # Create project
    $projectType = if ($layer -eq "Api") { "classlib" } else { "classlib" }
    dotnet new $projectType -n "$ModuleName.$layer" -o $layerPath -f net8.0
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Created $ModuleName.$layer"
    } else {
        Write-Error "Failed to create $ModuleName.$layer"
        exit 1
    }
}

# Create folder structure for each layer
Write-Info "Creating folder structure..."

# Domain layer folders
$domainPath = Join-Path $modulePath "$ModuleName.Domain"
@("Entities", "ValueObjects", "Events", "Repositories", "Exceptions") | ForEach-Object {
    $folderPath = Join-Path $domainPath $_
    New-Item -ItemType Directory -Path $folderPath -Force | Out-Null
}
Write-Success "Created Domain layer structure"

# Application layer folders
$applicationPath = Join-Path $modulePath "$ModuleName.Application"
@("Features", "DTOs", "Validators", "Mappings", "Contracts") | ForEach-Object {
    $folderPath = Join-Path $applicationPath $_
    New-Item -ItemType Directory -Path $folderPath -Force | Out-Null
}
Write-Success "Created Application layer structure"

# Infrastructure layer folders
$infrastructurePath = Join-Path $modulePath "$ModuleName.Infrastructure"
@("Persistence", "Persistence/Configurations", "Repositories", "Services") | ForEach-Object {
    $folderPath = Join-Path $infrastructurePath $_
    New-Item -ItemType Directory -Path $folderPath -Force | Out-Null
}
Write-Success "Created Infrastructure layer structure"

# Api layer folders
$apiPath = Join-Path $modulePath "$ModuleName.Api"
@("Controllers", "Extensions") | ForEach-Object {
    $folderPath = Join-Path $apiPath $_
    New-Item -ItemType Directory -Path $folderPath -Force | Out-Null
}
Write-Success "Created Api layer structure"

# Create DbContext template
$dbContextContent = @"
using Microsoft.EntityFrameworkCore;

namespace $ModuleName.Infrastructure.Persistence;

public class ${ModuleName}DbContext : DbContext
{
    public ${ModuleName}DbContext(DbContextOptions<${ModuleName}DbContext> options)
        : base(options)
    {
    }

    // Add DbSets here
    // public DbSet<Entity> Entities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(${ModuleName}DbContext).Assembly);
        
        // Set default schema (optional)
        modelBuilder.HasDefaultSchema("$($ModuleName.ToLower())");
    }
}
"@

$dbContextPath = Join-Path $infrastructurePath "Persistence/${ModuleName}DbContext.cs"
Set-Content -Path $dbContextPath -Value $dbContextContent
Write-Success "Created DbContext template"

# Create module registration extension
$moduleExtensionContent = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using $ModuleName.Infrastructure.Persistence;

namespace $ModuleName.Api.Extensions;

public static class ${ModuleName}ModuleExtensions
{
    public static IServiceCollection Add${ModuleName}Module(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        var connectionString = configuration.GetConnectionString("$ModuleName")
            ?? configuration.GetConnectionString("Default");
            
        services.AddDbContext<${ModuleName}DbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        // services.AddScoped<IRepository, Repository>();

        // Register MediatR handlers from Application layer
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(${ModuleName}ModuleExtensions).Assembly));

        // Register AutoMapper profiles
        // services.AddAutoMapper(typeof(${ModuleName}ModuleExtensions).Assembly);

        return services;
    }
}
"@

$moduleExtensionPath = Join-Path $apiPath "Extensions/${ModuleName}ModuleExtensions.cs"
Set-Content -Path $moduleExtensionPath -Value $moduleExtensionContent
Write-Success "Created module registration extension"

# Create README for the module
$readmeContent = @"
# $ModuleName Module

## Overview
This module handles [describe the business capability].

## Structure

### Domain Layer ($ModuleName.Domain)
- **Entities**: Core business entities
- **ValueObjects**: Immutable value objects
- **Events**: Domain events
- **Repositories**: Repository interfaces
- **Exceptions**: Domain-specific exceptions

### Application Layer ($ModuleName.Application)
- **Features**: Use cases organized by feature
- **DTOs**: Data transfer objects
- **Validators**: Input validation rules
- **Mappings**: Object mapping profiles
- **Contracts**: Service contracts

### Infrastructure Layer ($ModuleName.Infrastructure)
- **Persistence**: Database context and configurations
- **Repositories**: Repository implementations
- **Services**: External service integrations

### Api Layer ($ModuleName.Api)
- **Controllers**: HTTP endpoints
- **Extensions**: Module registration and configuration

## Dependencies

### Domain
- No external dependencies (pure business logic)

### Application
- Domain layer
- MediatR
- FluentValidation
- AutoMapper

### Infrastructure
- Application layer
- Entity Framework Core
- External service SDKs

### Api
- Application layer
- ASP.NET Core

## Usage

Register this module in the main API project:

\`\`\`csharp
builder.Services.Add${ModuleName}Module(builder.Configuration);
\`\`\`

Add connection string to appsettings.json:

\`\`\`json
{
  "ConnectionStrings": {
    "$ModuleName": "Server=...;Database=ProductManager.$ModuleName;..."
  }
}
\`\`\`

## Database Migrations

\`\`\`bash
# Add migration
dotnet ef migrations add InitialCreate \\
    --project src/Modules/$ModuleName/$ModuleName.Infrastructure \\
    --context ${ModuleName}DbContext

# Update database
dotnet ef database update \\
    --project src/Modules/$ModuleName/$ModuleName.Infrastructure \\
    --context ${ModuleName}DbContext
\`\`\`
"@

$readmePath = Join-Path $modulePath "README.md"
Set-Content -Path $readmePath -Value $readmeContent
Write-Success "Created module README"

# Add project references
Write-Info "Adding project references..."

# Application references Domain
dotnet add "$applicationPath/$ModuleName.Application.csproj" reference "$domainPath/$ModuleName.Domain.csproj"

# Infrastructure references Application
dotnet add "$infrastructurePath/$ModuleName.Infrastructure.csproj" reference "$applicationPath/$ModuleName.Application.csproj"

# Api references Infrastructure
dotnet add "$apiPath/$ModuleName.Api.csproj" reference "$infrastructurePath/$ModuleName.Infrastructure.csproj"

Write-Success "Added project references"

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Module Creation Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Module: $ModuleName" -ForegroundColor Yellow
Write-Host "Location: $modulePath" -ForegroundColor Yellow
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Add entities to $ModuleName.Domain/Entities" -ForegroundColor White
Write-Host "2. Create repository interfaces in $ModuleName.Domain/Repositories" -ForegroundColor White
Write-Host "3. Implement use cases in $ModuleName.Application/Features" -ForegroundColor White
Write-Host "4. Implement repositories in $ModuleName.Infrastructure/Repositories" -ForegroundColor White
Write-Host "5. Create controllers in $ModuleName.Api/Controllers" -ForegroundColor White
Write-Host "6. Register module in main API: builder.Services.Add${ModuleName}Module()" -ForegroundColor White
Write-Host ""
