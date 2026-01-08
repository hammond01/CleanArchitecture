# Refactoring Scripts

This directory contains PowerShell scripts to help with the Modular Monolith refactoring process.

## Available Scripts

### 1. create-module.ps1

**Purpose**: Creates a new module with complete Clean Architecture structure.

**Usage**:
```powershell
.\.scripts\create-module.ps1 -ModuleName "YourModule"
```

**What it creates**:
- Module folder structure with 4 layers (Domain, Application, Infrastructure, Api)
- Proper project references between layers
- DbContext template
- Module registration extension
- Folder structure for each layer
- Module README

**Example**:
```powershell
.\.scripts\create-module.ps1 -ModuleName "Catalog"
```

**Output**:
```
src/Modules/Catalog/
├── Catalog.Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   ├── Repositories/
│   └── Exceptions/
├── Catalog.Application/
│   ├── Features/
│   ├── DTOs/
│   ├── Validators/
│   ├── Mappings/
│   └── Contracts/
├── Catalog.Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   └── CatalogDbContext.cs
│   ├── Repositories/
│   └── Services/
├── Catalog.Api/
│   ├── Controllers/
│   └── Extensions/
│       └── CatalogModuleExtensions.cs
└── README.md
```

---

### 2. validate-architecture.ps1

**Purpose**: Validates that modules follow Modular Monolith architecture principles.

**Usage**:
```powershell
# Validate all modules
.\.scripts\validate-architecture.ps1

# Validate specific module
.\.scripts\validate-architecture.ps1 -ModuleName "Catalog"
```

**What it checks**:
- ✅ Module structure (all 4 layers exist)
- ✅ Domain layer has no infrastructure dependencies
- ✅ Application layer references Domain but not Infrastructure
- ✅ Infrastructure layer references Application
- ✅ DbContext exists and has schema configuration
- ✅ Module registration extension exists
- ✅ No cross-module dependency violations
- ✅ Test projects exist

**Example output**:
```
=== Validating Catalog Module Structure ===
✓ Catalog.Domain exists
✓ Catalog.Application exists
✓ Catalog.Infrastructure exists
✓ Catalog.Api exists

=== Validating Catalog Domain Layer ===
✓ Domain/Entities exists
✓ Domain/Repositories exists
✓ Domain layer has no infrastructure dependencies

=== Validation Summary ===
Successes: 25
Warnings: 3
Violations: 0

✓ Architecture validation passed!
```

---

### 3. migrate-all-modules.ps1 (To be created)

**Purpose**: Handles database migrations for all modules.

**Usage**:
```powershell
# Add migrations for all modules
.\.scripts\migrate-all-modules.ps1 -Operation "add"

# Update all databases
.\.scripts\migrate-all-modules.ps1 -Operation "update"

# Remove last migration from all modules
.\.scripts\migrate-all-modules.ps1 -Operation "remove"
```

**Create this script**:
```powershell
param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("add", "update", "remove")]
    [string]$Operation = "update"
)

$modules = @("Catalog", "Sales", "Customers", "Suppliers", "Geography")

foreach ($module in $modules) {
    Write-Host "Processing $module module..." -ForegroundColor Cyan
    
    $infraProject = "src/Modules/$module/$module.Infrastructure"
    $apiProject = "src/Api/ProductManager.Api"
    $context = "${module}DbContext"
    
    switch ($Operation) {
        "add" {
            dotnet ef migrations add InitialCreate `
                --project $infraProject `
                --startup-project $apiProject `
                --context $context `
                --output-dir Persistence/Migrations
        }
        "update" {
            dotnet ef database update `
                --project $infraProject `
                --startup-project $apiProject `
                --context $context
        }
        "remove" {
            dotnet ef migrations remove `
                --project $infraProject `
                --startup-project $apiProject `
                --context $context
        }
    }
}
```

---

### 4. test-all-modules.ps1 (To be created)

**Purpose**: Runs tests for all modules.

**Usage**:
```powershell
# Run all tests
.\.scripts\test-all-modules.ps1

# Run only unit tests
.\.scripts\test-all-modules.ps1 -TestType "unit"

# Run only integration tests
.\.scripts\test-all-modules.ps1 -TestType "integration"
```

**Create this script**:
```powershell
param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("all", "unit", "integration", "e2e")]
    [string]$TestType = "all"
)

$modules = @("Catalog", "Sales", "Customers", "Suppliers", "Geography")

if ($TestType -eq "all" -or $TestType -eq "unit") {
    Write-Host "`nRunning Unit Tests..." -ForegroundColor Cyan
    foreach ($module in $modules) {
        $testProject = "tests/UnitTests/$module.UnitTests"
        if (Test-Path $testProject) {
            Write-Host "Testing $module..." -ForegroundColor Yellow
            dotnet test $testProject --verbosity minimal
        }
    }
}

if ($TestType -eq "all" -or $TestType -eq "integration") {
    Write-Host "`nRunning Integration Tests..." -ForegroundColor Cyan
    foreach ($module in $modules) {
        $testProject = "tests/IntegrationTests/$module.IntegrationTests"
        if (Test-Path $testProject) {
            Write-Host "Testing $module..." -ForegroundColor Yellow
            dotnet test $testProject --verbosity minimal
        }
    }
}

if ($TestType -eq "all" -or $TestType -eq "e2e") {
    Write-Host "`nRunning E2E Tests..." -ForegroundColor Cyan
    $testProject = "tests/E2ETests/Api.E2ETests"
    if (Test-Path $testProject) {
        dotnet test $testProject --verbosity minimal
    }
}
```

---

### 5. build-all-modules.ps1 (To be created)

**Purpose**: Builds all modules and checks for compilation errors.

**Usage**:
```powershell
.\.scripts\build-all-modules.ps1
```

**Create this script**:
```powershell
$modules = @("Catalog", "Sales", "Customers", "Suppliers", "Geography")
$buildFailed = $false

Write-Host "Building BuildingBlocks..." -ForegroundColor Cyan
dotnet build src/BuildingBlocks/BuildingBlocks.Domain
dotnet build src/BuildingBlocks/BuildingBlocks.Application
dotnet build src/BuildingBlocks/BuildingBlocks.Infrastructure
dotnet build src/BuildingBlocks/BuildingBlocks.Api

foreach ($module in $modules) {
    Write-Host "`nBuilding $module module..." -ForegroundColor Cyan
    
    $modulePath = "src/Modules/$module"
    
    if (Test-Path $modulePath) {
        dotnet build "$modulePath/$module.Domain"
        if ($LASTEXITCODE -ne 0) { $buildFailed = $true }
        
        dotnet build "$modulePath/$module.Application"
        if ($LASTEXITCODE -ne 0) { $buildFailed = $true }
        
        dotnet build "$modulePath/$module.Infrastructure"
        if ($LASTEXITCODE -ne 0) { $buildFailed = $true }
        
        dotnet build "$modulePath/$module.Api"
        if ($LASTEXITCODE -ne 0) { $buildFailed = $true }
    } else {
        Write-Warning "$module module not found (may not be migrated yet)"
    }
}

Write-Host "`nBuilding main API..." -ForegroundColor Cyan
dotnet build src/Api/ProductManager.Api

if ($buildFailed) {
    Write-Host "`n✗ Build failed!" -ForegroundColor Red
    exit 1
} else {
    Write-Host "`n✓ Build successful!" -ForegroundColor Green
    exit 0
}
```

---

## Workflow Examples

### Creating a New Module

```powershell
# 1. Create module structure
.\.scripts\create-module.ps1 -ModuleName "Catalog"

# 2. Add your entities, repositories, etc.
# ... (manual code migration)

# 3. Validate architecture
.\.scripts\validate-architecture.ps1 -ModuleName "Catalog"

# 4. Build the module
dotnet build src/Modules/Catalog

# 5. Add database migration
dotnet ef migrations add InitialCreate `
    --project src/Modules/Catalog/Catalog.Infrastructure `
    --startup-project src/Api/ProductManager.Api `
    --context CatalogDbContext

# 6. Update database
dotnet ef database update `
    --project src/Modules/Catalog/Catalog.Infrastructure `
    --startup-project src/Api/ProductManager.Api `
    --context CatalogDbContext

# 7. Run tests
dotnet test tests/UnitTests/Catalog.UnitTests
dotnet test tests/IntegrationTests/Catalog.IntegrationTests
```

### Complete Migration Workflow

```powershell
# 1. Create all modules
.\.scripts\create-module.ps1 -ModuleName "Catalog"
.\.scripts\create-module.ps1 -ModuleName "Sales"
.\.scripts\create-module.ps1 -ModuleName "Customers"
.\.scripts\create-module.ps1 -ModuleName "Suppliers"
.\.scripts\create-module.ps1 -ModuleName "Geography"

# 2. Migrate code for each module
# ... (manual migration following MODULE_MIGRATION_GUIDE.md)

# 3. Validate all modules
.\.scripts\validate-architecture.ps1

# 4. Build everything
.\.scripts\build-all-modules.ps1

# 5. Add migrations for all modules
.\.scripts\migrate-all-modules.ps1 -Operation "add"

# 6. Update all databases
.\.scripts\migrate-all-modules.ps1 -Operation "update"

# 7. Run all tests
.\.scripts\test-all-modules.ps1
```

---

## Prerequisites

### PowerShell Version
These scripts require **PowerShell 7+**. Check your version:
```powershell
$PSVersionTable.PSVersion
```

If you need to install PowerShell 7:
```powershell
# Windows
winget install Microsoft.PowerShell

# Or download from: https://github.com/PowerShell/PowerShell/releases
```

### Execution Policy
You may need to set the execution policy to run these scripts:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### .NET CLI
Ensure you have .NET 8 SDK installed:
```bash
dotnet --version
```

### Entity Framework Tools
For migration scripts:
```bash
dotnet tool install --global dotnet-ef
```

---

## Troubleshooting

### Script won't run
**Error**: "cannot be loaded because running scripts is disabled"

**Solution**:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Module creation fails
**Error**: "dotnet command not found"

**Solution**: Install .NET 8 SDK from https://dotnet.microsoft.com/download

### Migration fails
**Error**: "No DbContext found"

**Solution**: Ensure the startup project (main API) references the Infrastructure project

### Validation fails
**Error**: "Architecture validation failed"

**Solution**: Review the violations listed and fix them according to the architecture rules

---

## Best Practices

1. **Always validate** after creating or modifying a module:
   ```powershell
   .\.scripts\validate-architecture.ps1 -ModuleName "YourModule"
   ```

2. **Build before testing**:
   ```powershell
   dotnet build
   dotnet test
   ```

3. **Use version control**:
   ```bash
   git add .
   git commit -m "feat: add Catalog module"
   ```

4. **Test incrementally**: Don't create all modules at once. Create, test, commit, then move to the next.

5. **Keep scripts updated**: If you find issues or improvements, update the scripts and document them.

---

## Contributing

If you create new helpful scripts, please:
1. Add them to this directory
2. Update this README
3. Follow the same naming convention
4. Include help documentation in the script
5. Test thoroughly before committing

---

## Related Documentation

- [REFACTORING_GUIDE.md](../REFACTORING_GUIDE.md) - Complete refactoring guide
- [REFACTORING_PLAN.md](../REFACTORING_PLAN.md) - Detailed refactoring plan
- [MODULE_MIGRATION_GUIDE.md](../MODULE_MIGRATION_GUIDE.md) - Module migration steps
- [ARCHITECTURE.md](../ARCHITECTURE.md) - Target architecture principles
