# PostgreSQL Migration Notes

## Baseline Status

- Provider target is PostgreSQL (`Npgsql`) for API modules and DbMigrator.
- Fresh baseline migrations were generated with Postgres markers:
  - `InitialIdentity_PostgresBaseline`
  - `InitialCatalog_PostgresBaseline`
  - `InitialAuditing_PostgresBaseline`
- Migration history is scoped per schema:
  - `identity.__EFMigrationsHistory`
  - `catalog.__EFMigrationsHistory`
  - `auditing.__EFMigrationsHistory`

## Local Run

1. Set connection string (if not using defaults):
   - `ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=CleanArchitecture;Username=postgres;Password=postgres`
2. Apply migrations:
   - `dotnet run --project src/ModularMonolith/DbMigrator`
3. Start API:
   - `dotnet run --project src/ModularMonolith/CleanArchitecture.Api`

## Docker Compose Run (Local/Staging-like)

- Bring up PostgreSQL, DbMigrator, and API:
  - `docker compose up --build`
- Override DB credentials/database if needed:
  - `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`
- Optional seed toggle for migrator:
  - `MIGRATOR_SEED_DATA=true|false`
