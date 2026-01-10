<#
.SYNOPSIS
    Validates the Modular Monolith architecture structure

.DESCRIPTION
    This script validates that the migrated modules follow the correct
    Modular Monolith architecture patterns and identifies any violations.

.PARAMETER ModuleName
    Optional. Validate a specific module. If not provided, validates all modules.

.EXAMPLE
    .\validate-architecture.ps1
    Validates all modules

.EXAMPLE
    .\validate-architecture.ps1 -ModuleName "Catalog"
    Validates only the Catalog module
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$ModuleName = ""
)

# Color output functions
function Write-Success { param($Message) Write-Host "✓ $Message" -ForegroundColor Green }
function Write-Info { param($Message) Write-Host "ℹ $Message" -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host "⚠ $Message" -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host "✗ $Message" -ForegroundColor Red }
function Write-Header { param($Message) Write-Host "`n=== $Message ===" -ForegroundColor Magenta }

$script:violations = @()
$script:warnings = @()
$script:successes = @()

function Add-Violation {
    param([string]$Message)
    $script:violations += $Message
    Write-Error $Message
}

function Add-Warning {
    param([string]$Message)
    $script:warnings += $Message
    Write-Warning $Message
}

function Add-Success {
    param([string]$Message)
    $script:successes += $Message
    Write-Success $Message
}

function Test-ModuleStructure {
    param([string]$Module)
    
    Write-Header "Validating $Module Module Structure"
    
    $modulePath = "src/Modules/$Module"
    
    if (-not (Test-Path $modulePath)) {
        Add-Violation "Module directory not found: $modulePath"
        return $false
    }
    
    # Check for required layers
    $requiredLayers = @("Domain", "Application", "Infrastructure", "Api")
    $allLayersExist = $true
    
    foreach ($layer in $requiredLayers) {
        $layerPath = "$modulePath/$Module.$layer"
        if (Test-Path $layerPath) {
            Add-Success "$Module.$layer exists"
        } else {
            Add-Violation "$Module.$layer is missing"
            $allLayersExist = $false
        }
    }
    
    return $allLayersExist
}

function Test-DomainLayer {
    param([string]$Module)
    
    Write-Header "Validating $Module Domain Layer"
    
    $domainPath = "src/Modules/$Module/$Module.Domain"
    
    # Check for required folders
    $requiredFolders = @("Entities", "Repositories")
    foreach ($folder in $requiredFolders) {
        $folderPath = "$domainPath/$folder"
        if (Test-Path $folderPath) {
            Add-Success "Domain/$folder exists"
        } else {
            Add-Warning "Domain/$folder is missing (may be empty)"
        }
    }
    
    # Check for no infrastructure dependencies
    $csprojPath = "$domainPath/$Module.Domain.csproj"
    if (Test-Path $csprojPath) {
        $content = Get-Content $csprojPath -Raw
        
        if ($content -match "EntityFrameworkCore" -or 
            $content -match "SqlServer" -or 
            $content -match "Infrastructure") {
            Add-Violation "Domain layer has infrastructure dependencies (violates Clean Architecture)"
        } else {
            Add-Success "Domain layer has no infrastructure dependencies"
        }
    }
}

function Test-ApplicationLayer {
    param([string]$Module)
    
    Write-Header "Validating $Module Application Layer"
    
    $applicationPath = "src/Modules/$Module/$Module.Application"
    
    # Check for required folders
    $requiredFolders = @("Features", "DTOs")
    foreach ($folder in $requiredFolders) {
        $folderPath = "$applicationPath/$folder"
        if (Test-Path $folderPath) {
            Add-Success "Application/$folder exists"
        } else {
            Add-Warning "Application/$folder is missing (may be empty)"
        }
    }
    
    # Check for proper dependencies
    $csprojPath = "$applicationPath/$Module.Application.csproj"
    if (Test-Path $csprojPath) {
        $content = Get-Content $csprojPath -Raw
        
        # Should reference Domain
        if ($content -match "$Module.Domain") {
            Add-Success "Application layer references Domain layer"
        } else {
            Add-Violation "Application layer does not reference Domain layer"
        }
        
        # Should NOT reference Infrastructure
        if ($content -match "$Module.Infrastructure") {
            Add-Violation "Application layer references Infrastructure layer (violates dependency rule)"
        } else {
            Add-Success "Application layer does not reference Infrastructure layer"
        }
    }
}

function Test-InfrastructureLayer {
    param([string]$Module)
    
    Write-Header "Validating $Module Infrastructure Layer"
    
    $infrastructurePath = "src/Modules/$Module/$Module.Infrastructure"
    
    # Check for DbContext
    $dbContextPath = "$infrastructurePath/Persistence/${Module}DbContext.cs"
    if (Test-Path $dbContextPath) {
        Add-Success "DbContext exists"
        
        # Check for proper schema configuration
        $content = Get-Content $dbContextPath -Raw
        if ($content -match "HasDefaultSchema") {
            Add-Success "DbContext has schema configuration"
        } else {
            Add-Warning "DbContext does not have schema configuration (recommended for isolation)"
        }
    } else {
        Add-Violation "DbContext not found at $dbContextPath"
    }
    
    # Check for Configurations folder
    $configurationsPath = "$infrastructurePath/Persistence/Configurations"
    if (Test-Path $configurationsPath) {
        Add-Success "Entity configurations folder exists"
    } else {
        Add-Warning "Entity configurations folder is missing"
    }
    
    # Check for proper dependencies
    $csprojPath = "$infrastructurePath/$Module.Infrastructure.csproj"
    if (Test-Path $csprojPath) {
        $content = Get-Content $csprojPath -Raw
        
        # Should reference Application
        if ($content -match "$Module.Application") {
            Add-Success "Infrastructure layer references Application layer"
        } else {
            Add-Violation "Infrastructure layer does not reference Application layer"
        }
    }
}

function Test-ApiLayer {
    param([string]$Module)
    
    Write-Header "Validating $Module Api Layer"
    
    $apiPath = "src/Modules/$Module/$Module.Api"
    
    # Check for Controllers folder
    $controllersPath = "$apiPath/Controllers"
    if (Test-Path $controllersPath) {
        Add-Success "Controllers folder exists"
        
        # Count controllers
        $controllers = Get-ChildItem "$controllersPath/*.cs" -ErrorAction SilentlyContinue
        if ($controllers.Count -gt 0) {
            Add-Success "Found $($controllers.Count) controller(s)"
        } else {
            Add-Warning "No controllers found"
        }
    } else {
        Add-Warning "Controllers folder is missing"
    }
    
    # Check for module registration
    $extensionsPath = "$apiPath/Extensions/${Module}ModuleExtensions.cs"
    if (Test-Path $extensionsPath) {
        Add-Success "Module registration extension exists"
        
        # Check for proper registration methods
        $content = Get-Content $extensionsPath -Raw
        if ($content -match "Add${Module}Module") {
            Add-Success "Module has Add${Module}Module method"
        } else {
            Add-Violation "Module registration does not have Add${Module}Module method"
        }
        
        if ($content -match "AddDbContext") {
            Add-Success "Module registers DbContext"
        } else {
            Add-Warning "Module does not register DbContext"
        }
    } else {
        Add-Violation "Module registration extension not found"
    }
}

function Test-CrossModuleDependencies {
    param([string]$Module)
    
    Write-Header "Validating $Module Cross-Module Dependencies"
    
    $modulePath = "src/Modules/$Module"
    
    # Get all .cs files in the module
    $csFiles = Get-ChildItem -Path $modulePath -Filter "*.cs" -Recurse
    
    # List of other modules to check for violations
    $otherModules = @("Catalog", "Sales", "Customers", "Suppliers", "Geography") | Where-Object { $_ -ne $Module }
    
    $hasViolations = $false
    foreach ($file in $csFiles) {
        $content = Get-Content $file.FullName -Raw
        
        foreach ($otherModule in $otherModules) {
            # Check for direct references to other module's internal types
            if ($content -match "using $otherModule\.(Domain|Application|Infrastructure)\.") {
                Add-Violation "File $($file.Name) references internal types from $otherModule module"
                $hasViolations = $true
            }
        }
    }
    
    if (-not $hasViolations) {
        Add-Success "No cross-module dependency violations found"
    }
}

function Test-BuildingBlocks {
    Write-Header "Validating BuildingBlocks"
    
    $buildingBlocksPath = "src/BuildingBlocks"
    
    if (-not (Test-Path $buildingBlocksPath)) {
        Add-Violation "BuildingBlocks directory not found"
        return
    }
    
    $requiredProjects = @("BuildingBlocks.Domain", "BuildingBlocks.Application", 
                          "BuildingBlocks.Infrastructure", "BuildingBlocks.Api")
    
    foreach ($project in $requiredProjects) {
        $projectPath = "$buildingBlocksPath/$project"
        if (Test-Path $projectPath) {
            Add-Success "$project exists"
        } else {
            Add-Violation "$project is missing"
        }
    }
}

function Test-CompositionRoot {
    Write-Header "Validating API Composition Root"
    
    $apiPath = "src/Api/ProductManager.Api"
    
    if (-not (Test-Path $apiPath)) {
        Add-Warning "Main API project not found at $apiPath (may not be migrated yet)"
        return
    }
    
    # Check Program.cs
    $programPath = "$apiPath/Program.cs"
    if (Test-Path $programPath) {
        Add-Success "Program.cs exists"
        
        $content = Get-Content $programPath -Raw
        
        # Check for module registrations
        $modules = @("Catalog", "Sales", "Customers", "Suppliers", "Geography")
        foreach ($module in $modules) {
            if ($content -match "Add${module}Module") {
                Add-Success "Program.cs registers $module module"
            } else {
                Add-Warning "Program.cs does not register $module module (may not be migrated yet)"
            }
        }
    } else {
        Add-Warning "Program.cs not found"
    }
}

function Test-TestStructure {
    param([string]$Module)
    
    Write-Header "Validating $Module Test Structure"
    
    # Check for unit tests
    $unitTestPath = "tests/UnitTests/$Module.UnitTests"
    if (Test-Path $unitTestPath) {
        Add-Success "Unit tests project exists"
    } else {
        Add-Warning "Unit tests project not found (may not be migrated yet)"
    }
    
    # Check for integration tests
    $integrationTestPath = "tests/IntegrationTests/$Module.IntegrationTests"
    if (Test-Path $integrationTestPath) {
        Add-Success "Integration tests project exists"
    } else {
        Add-Warning "Integration tests project not found (may not be migrated yet)"
    }
}

# Main execution
Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Modular Monolith Architecture Validation Tool            ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

# Determine which modules to validate
$modulesToValidate = @()
if ($ModuleName) {
    $modulesToValidate = @($ModuleName)
    Write-Info "Validating module: $ModuleName"
} else {
    $modulesToValidate = @("Catalog", "Sales", "Customers", "Suppliers", "Geography")
    Write-Info "Validating all modules"
}

# Validate BuildingBlocks first
Test-BuildingBlocks

# Validate each module
foreach ($module in $modulesToValidate) {
    if (Test-ModuleStructure $module) {
        Test-DomainLayer $module
        Test-ApplicationLayer $module
        Test-InfrastructureLayer $module
        Test-ApiLayer $module
        Test-CrossModuleDependencies $module
        Test-TestStructure $module
    }
}

# Validate composition root
Test-CompositionRoot

# Summary
Write-Host "`n╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Validation Summary                                        ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝`n" -ForegroundColor Cyan

Write-Host "Successes: $($script:successes.Count)" -ForegroundColor Green
Write-Host "Warnings:  $($script:warnings.Count)" -ForegroundColor Yellow
Write-Host "Violations: $($script:violations.Count)" -ForegroundColor Red

if ($script:violations.Count -eq 0) {
    Write-Host "`n✓ Architecture validation passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`n✗ Architecture validation failed with $($script:violations.Count) violation(s)" -ForegroundColor Red
    Write-Host "`nViolations:" -ForegroundColor Red
    foreach ($violation in $script:violations) {
        Write-Host "  - $violation" -ForegroundColor Red
    }
    exit 1
}
