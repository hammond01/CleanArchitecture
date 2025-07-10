using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProductManager.Infrastructure.Configuration;
using ProductManager.Persistence;
namespace ProductManager.Api.Extensions;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{GetEnvironmentName()}.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        // ONLY use AppSettings - No fallback to ConnectionStrings
        var connectionString = configuration[AppSettings.ConfigPaths.DatabasePaths.DefaultConnection];

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Database connection string is required. Please set '{AppSettings.ConfigPaths.DatabasePaths.DefaultConnection}' in appsettings.json");
        }

        optionsBuilder.UseSqlServer(connectionString);


        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string GetEnvironmentName()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.IsNullOrEmpty(env) ? "Production" : env;
    }
}
