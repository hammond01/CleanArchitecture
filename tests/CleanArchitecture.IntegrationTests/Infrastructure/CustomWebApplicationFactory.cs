using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Catalog.Infrastructure.Persistence;
using Identity.Infrastructure.Persistence;
using Auditing.Infrastructure.Persistence;

namespace CleanArchitecture.IntegrationTests.Infrastructure;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// Configures in-memory databases and test services
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the real DbContexts
            services.RemoveAll(typeof(DbContextOptions<CatalogDbContext>));
            services.RemoveAll(typeof(DbContextOptions<IdentityDbContext>));
            services.RemoveAll(typeof(DbContextOptions<AuditingDbContext>));

            // Add in-memory databases for testing
            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestCatalogDb");
            });

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestIdentityDb");
            });

            services.AddDbContext<AuditingDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestAuditingDb");
            });

            // Ensure databases are created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var catalogDb = scopedServices.GetRequiredService<CatalogDbContext>();
            var identityDb = scopedServices.GetRequiredService<IdentityDbContext>();
            var auditingDb = scopedServices.GetRequiredService<AuditingDbContext>();

            catalogDb.Database.EnsureCreated();
            identityDb.Database.EnsureCreated();
            auditingDb.Database.EnsureCreated();
        });

        builder.UseEnvironment("Test");
    }
}
