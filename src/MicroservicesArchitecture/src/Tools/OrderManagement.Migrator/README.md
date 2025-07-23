# Order Management Database Migrator

A command-line tool for managing database migrations and seeding data for the Order Management service.

## Features

- 🔄 Apply database migrations
- 🌱 Seed sample data
- 📊 Show database information and statistics
- 🗑️ Drop database (with confirmation)
- 🔄 Reset database completely
- 🧹 Clear all data
- ⚙️ Configurable via appsettings.json

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)

### Installation

1. Navigate to the migrator directory:
```bash
cd src/Tools/OrderManagement.Migrator
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Update connection string in `appsettings.json` if needed

### Usage

#### Using Batch/Shell Scripts (Recommended)

**Windows:**
```cmd
# Apply migrations and seed data
migrate.bat

# Show help
migrate.bat help

# Show database info
migrate.bat info

# Reset database
migrate.bat reset
```

**Linux/macOS:**
```bash
# Make script executable
chmod +x migrate.sh

# Apply migrations and seed data
./migrate.sh

# Show help
./migrate.sh help

# Show database info
./migrate.sh info

# Reset database
./migrate.sh reset
```

#### Using dotnet run

```bash
# Apply migrations and seed data
dotnet run migrate

# Seed sample data only
dotnet run seed

# Show database information
dotnet run info

# Drop database (WARNING: destructive)
dotnet run drop

# Reset database completely
dotnet run reset

# Clear all data
dotnet run clear

# Show help
dotnet run help
```

## Commands

| Command | Alias | Description |
|---------|-------|-------------|
| `migrate` | `m` | Apply pending migrations and optionally seed data |
| `seed` | `s` | Seed database with sample data |
| `info` | `i` | Show database and data information |
| `drop` | `d` | Drop the database (WARNING: destructive) |
| `reset` | `r` | Drop and recreate database with migrations and seed data |
| `clear` | `c` | Clear all data from database (WARNING: destructive) |
| `help` | `h`, `?` | Show help message |

## Configuration

### Connection String

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderManagementDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
}
```

### Migration Settings

Configure migration behavior:

```json
{
  "Migration": {
    "AutoApplyMigrations": true,
    "SeedData": true,
    "DropDatabaseIfExists": false
  }
}
```

### Environment-Specific Settings

Create `appsettings.Development.json` for development settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderManagementDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "Logging": {
    "EnableSensitiveDataLogging": true,
    "EnableDetailedErrors": true
  }
}
```

Set environment variable:
```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Development

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Development
```

## Sample Data

The migrator includes sample data with:

- 6 sample orders in different statuses (Pending, Confirmed, Processing, Shipped, Delivered, Cancelled)
- Various products and customers
- Realistic order scenarios

### Sample Orders

1. **Order 1** - Pending: Laptop and mouse
2. **Order 2** - Confirmed: Smartphone and case
3. **Order 3** - Processing: Tablet and stand
4. **Order 4** - Shipped: Headphones and USB cable
5. **Order 5** - Delivered: Keyboard and monitor
6. **Order 6** - Cancelled: Webcam and microphone

## Creating New Migrations

To create a new migration, use Entity Framework tools from the Infrastructure project:

```bash
# Navigate to Infrastructure project
cd ../../Services/OrderManagement/OrderManagement.Infrastructure

# Add migration
dotnet ef migrations add YourMigrationName --startup-project ../OrderManagement.API

# Apply migration using migrator
cd ../../../Tools/OrderManagement.Migrator
dotnet run migrate
```

## Troubleshooting

### Common Issues

1. **Connection String Issues**
   - Verify SQL Server is running
   - Check connection string format
   - Ensure database permissions

2. **Migration Errors**
   - Check for pending migrations: `dotnet run info`
   - Verify Entity Framework tools are installed
   - Check for conflicting migrations

3. **Seeding Errors**
   - Ensure database is migrated first
   - Check for existing data conflicts
   - Verify sample data integrity

### Logging

Enable detailed logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    },
    "EnableSensitiveDataLogging": true,
    "EnableDetailedErrors": true
  }
}
```

## Examples

### First Time Setup

```bash
# 1. Apply migrations and seed data
dotnet run migrate

# 2. Verify setup
dotnet run info
```

### Development Workflow

```bash
# 1. Reset database for clean state
dotnet run reset

# 2. Check database status
dotnet run info

# 3. Add more sample data if needed
dotnet run seed
```

### Production Deployment

```bash
# 1. Apply migrations only (no seeding)
# Set Migration:SeedData to false in appsettings.json
dotnet run migrate

# 2. Verify deployment
dotnet run info
```

## Security Notes

- Never run destructive commands (`drop`, `reset`, `clear`) in production
- Always backup production databases before migrations
- Use environment-specific connection strings
- Disable sensitive data logging in production
