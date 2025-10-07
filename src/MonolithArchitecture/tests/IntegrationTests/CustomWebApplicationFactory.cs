using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Storage;
using ProductManager.Persistence;
using Microsoft.Data.SqlClient;

namespace ProductManager.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async services =>
        {
            // Remove the app's ApplicationDbContext registration.
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove the DatabaseInitializer service to prevent migrations in tests
            var databaseInitializerDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDatabaseInitializer));

            if (databaseInitializerDescriptor != null)
            {
                services.Remove(databaseInitializerDescriptor);
            }

            // Use the existing database for testing
            var databaseName = $"ProductManager_Test_{Guid.NewGuid()}";

            // Add ApplicationDbContext using SQL Server for testing.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer($"Data Source=127.0.0.1;Initial Catalog={databaseName};Persist Security Info=True;User Id=sa;password=123456;TrustServerCertificate=true");
            });

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database
            // context (ApplicationDbContext).
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();

                try
                {
                    // Ensure the database is created
                    await dbContext.Database.EnsureCreatedAsync();

                    // Seed test data
                    await SeedTestDataAsync(scopedServices);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception
                    Console.WriteLine($"An error occurred setting up the database: {ex.Message}");
                }
            }
        });
    }

    private static async Task SeedTestDataAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Check if data already exists
        var existingCategories = await context.Categories.CountAsync();
        var existingProducts = await context.Products.CountAsync();

        if (existingCategories > 0 || existingProducts > 0)
        {
            Console.WriteLine($"Database already has {existingCategories} categories and {existingProducts} products. Skipping seeding.");
            return;
        }

        // Only seed if tables are empty
        // Seed categories
        var categories = new List<Categories>
        {
            new Categories
            {
                Id = "test-category-1",
                CategoryName = "Electronics",
                Description = "Electronic devices and gadgets"
            },
            new Categories
            {
                Id = "test-category-2",
                CategoryName = "Books",
                Description = "Books and publications"
            },
            new Categories
            {
                Id = "test-category-3",
                CategoryName = "Clothing",
                Description = "Clothing and apparel"
            }
        };
        await context.Categories.AddRangeAsync(categories);

        // Seed products
        var products = new List<Products>
        {
            new Products
            {
                Id = "test-product-1",
                ProductName = "iPhone 15",
                CategoryId = "test-category-1",
                UnitPrice = 999.99m,
                UnitsInStock = 50,
                UnitsOnOrder = 10,
                Discontinued = false,
                QuantityPerUnit = "1 unit",
                ReorderLevel = 5
            },
            new Products
            {
                Id = "test-product-2",
                ProductName = "MacBook Pro",
                CategoryId = "test-category-1",
                UnitPrice = 1999.99m,
                UnitsInStock = 25,
                UnitsOnOrder = 5,
                Discontinued = false,
                QuantityPerUnit = "1 unit",
                ReorderLevel = 3
            },
            new Products
            {
                Id = "test-product-3",
                ProductName = "Clean Code Book",
                CategoryId = "test-category-2",
                UnitPrice = 49.99m,
                UnitsInStock = 100,
                UnitsOnOrder = 20,
                Discontinued = false,
                QuantityPerUnit = "1 book",
                ReorderLevel = 10
            }
        };
        await context.Products.AddRangeAsync(products);

        await context.SaveChangesAsync();

        // Verify data was seeded
        var categoryCount = await context.Categories.CountAsync();
        var productCount = await context.Products.CountAsync();
        Console.WriteLine($"Seeded {categoryCount} categories and {productCount} products");
    }
}
