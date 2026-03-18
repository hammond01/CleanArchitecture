using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString(args);
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            builder => builder.MigrationsHistoryTable("__EFMigrationsHistory", "catalog"));

        return new CatalogDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString(string[] args)
    {
        var fromArgs = args.FirstOrDefault(static arg =>
            arg.StartsWith("--connection=", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(fromArgs))
        {
            return fromArgs["--connection=".Length..];
        }

        return Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=CleanArchitecture;Username=postgres;Password=postgres";
    }
}
