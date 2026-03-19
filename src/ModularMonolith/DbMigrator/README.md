# DbMigrator - Database Migration & Seeding Tool

> **🛠️ Professional database migration orchestrator** cho Clean Architecture project với support cho multiple DbContexts và independent schemas.

## 📋 Tổng Quan

DbMigrator là console application tự động hóa việc apply database migrations cho tất cả modules trong Clean Architecture solution. Thay vì phải chạy 3 lệnh `dotnet ef` riêng biệt, bạn chỉ cần chạy **1 command duy nhất**.

### ✨ Tính Năng

- ✅ **Orchestrate nhiều DbContexts** - Tự động migrate Identity, Auditing, Catalog theo thứ tự đúng
- ✅ **Health Checks** - Kiểm tra database connectivity trước khi migrate
- ✅ **Structured Logging** - Serilog với console + file output, emoji indicators
- ✅ **Idempotent** - An toàn chạy nhiều lần, chỉ apply pending migrations
- ✅ **Seed Data** - Infrastructure hỗ trợ seed initial data (configurable)
- ✅ **Error Handling** - Chi tiết error messages với stack trace

## 🚀 Cách Sử Dụng

### Development Environment

```powershell
# Từ thư mục root của solution
cd src/ModularMonolith/DbMigrator
dotnet run
```

### Production/CI-CD

```powershell
# Build release
dotnet build -c Release src/ModularMonolith/DbMigrator/DbMigrator.csproj

# Run với custom config
cd src/ModularMonolith/DbMigrator/bin/Release/net8.0
./DbMigrator --environment Production
```

### Docker Deployment

```dockerfile
# Trong Dockerfile của bạn
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . .
RUN dotnet publish src/ModularMonolith/DbMigrator/DbMigrator.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DbMigrator.dll"]
```

## ⚙️ Cấu Hình

### appsettings.json

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=CleanArchitecture;Username=postgres;Password=postgres"
    },
    "MigrationSettings": {
        "SeedData": true, // Có seed data sau migrations
        "CreateDatabaseIfNotExists": true, // Tự động tạo DB nếu chưa tồn tại
        "TimeoutSeconds": 300 // Timeout cho migrations
    }
}
```

### Environment Variables

```bash
# Override connection string
ConnectionStrings__DefaultConnection="Host=prod.database;Port=5432;Database=CleanArchitecture;Username=app;Password=secret"

# Override migration settings
MigrationSettings__SeedData=false
```

## 📊 Output Logging

DbMigrator sử dụng emoji để dễ theo dõi:

```
╔═══════════════════════════════════════════════╗
║   🛠️  Clean Architecture DB Migrator         ║
║   Database Migration & Seeding Tool          ║
╚═══════════════════════════════════════════════╝

🚀 Starting Database Migration Process...
Settings: SeedData=True, CreateDb=True, Timeout=300s

🔍 Checking database connectivity...
✅ Database connection successful

📦 Migrating Identity module (schema: identity)...
⏳ Applying 1 pending migrations: 20260318132414_InitialIdentity_PostgresBaseline
✅ Identity migrations applied

📦 Migrating Auditing module (schema: auditing)...
✅ Auditing is up to date (no pending migrations)

📦 Migrating Catalog module (schema: catalog)...
⏳ Applying 1 pending migrations: 20260318164001_InitialCatalog_PostgresSnakeCase
✅ Catalog migrations applied

🌱 Seeding initial data...
✅ Data seeding completed

✅ All migrations completed successfully!

🎉 Migration process completed successfully!
```

## 🏗️ Architecture

```
DbMigrator/
├── Program.cs                  # Entry point với Host configuration
├── DbMigrationService.cs       # Core orchestration logic
├── MigrationSettings.cs        # Configuration model
├── appsettings.json           # Default configuration
└── appsettings.Development.json
```

### Migration Sequence

1. **Database Connectivity Check** - Verify connection string
2. **Identity Module** → `identity` schema
3. **Auditing Module** → `auditing` schema
4. **Catalog Module** → `catalog` schema
5. **Seed Data** (if enabled)

## 🔧 Thêm Module Mới

Khi thêm module mới, update `DbMigrationService.cs`:

```csharp
// 1. Constructor - inject DbContext
private readonly NewModuleDbContext _newModuleDbContext;

public DbMigrationService(
    // ... existing contexts
    NewModuleDbContext newModuleDbContext)
{
    _newModuleDbContext = newModuleDbContext;
}

// 2. MigrateAsync - thêm migration step
await MigrateNewModuleAsync(cancellationToken);

// 3. Implement migration method
private async Task MigrateNewModuleAsync(CancellationToken ct)
{
    _logger.LogInformation("📦 Migrating NewModule (schema: newmodule)...");
    // ... same pattern as other modules
}
```

## 📝 CI/CD Integration

### GitHub Actions

```yaml
- name: Run Database Migrations
  run: |
      cd src/ModularMonolith/DbMigrator
      dotnet run --environment Production
  env:
      ConnectionStrings__DefaultConnection: ${{ secrets.DB_CONNECTION_STRING }}
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: "Run DB Migrations"
  inputs:
      command: "run"
      projects: "src/ModularMonolith/DbMigrator/DbMigrator.csproj"
      arguments: "--environment $(Environment)"
```

## 🐛 Troubleshooting

### Lỗi: "There is already an object named 'XXX'"

**Nguyên nhân:** Database có tables cũ nhưng MigrationsHistory table rỗng hoặc không đồng bộ.

**Giải pháp:**

```powershell
# Option 1: Drop và recreate database (Development only!)
psql -h localhost -U postgres -d postgres -c "DROP DATABASE IF EXISTS \"CleanArchitecture\";"
psql -h localhost -U postgres -d postgres -c "CREATE DATABASE \"CleanArchitecture\";"

# Option 2: Manually insert migration records vào __EFMigrationsHistory
```

### Lỗi: "Cannot connect to database"

**Kiểm tra:**

- Connection string trong `appsettings.json`
- PostgreSQL service đang chạy
- Firewall rules
- User permissions

## 📈 Best Practices

1. **Always backup production DB** trước khi run migrations
2. **Test trên staging** environment trước
3. **Use transactions** - EF Core migrations tự động wrap trong transaction
4. **Monitor logs** - Check file logs trong `logs/dbmigrator-*.log`
5. **Version control migrations** - Không delete migration files cũ

## 🔒 Security

- ⚠️ **KHÔNG commit** connection strings với credentials vào Git
- ✅ **Sử dụng** environment variables hoặc Azure Key Vault
- ✅ **Least privilege** - Migration user chỉ cần quyền DDL
- ✅ **Audit logs** - Tất cả migrations được log với timestamp

## 📚 Related Documentation

- [Entity Framework Core Migrations](https://learn.microsoft.com/ef/core/managing-schemas/migrations/)
- [PostgreSQL Migration Notes](../../../docs/POSTGRES_MIGRATION_NOTES.md)
- [Clean Architecture Project Structure](../../ARCHITECTURE.md)
- [Module Development Guide](../../README.md#-development-guide)

---

**Version:** 1.0.0
**Last Updated:** 2026-02-23
**Maintainer:** Clean Architecture Team
