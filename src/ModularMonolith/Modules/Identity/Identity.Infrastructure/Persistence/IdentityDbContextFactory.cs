using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Infrastructure.Persistence;

public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString(args);
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            builder => builder.MigrationsHistoryTable("__ef_migrations_history", "identity"))
            .UseSnakeCaseNamingConvention();

        return new IdentityDbContext(optionsBuilder.Options);
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

