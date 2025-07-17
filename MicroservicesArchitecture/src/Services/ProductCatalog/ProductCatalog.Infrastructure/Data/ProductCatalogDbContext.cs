using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Data;

/// <summary>
/// Product catalog database context
/// </summary>
public class ProductCatalogDbContext : DbContext
{
    public ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Supplier> Suppliers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(p => p.ProductId);
            entity.Property(p => p.ProductId).HasMaxLength(50).IsRequired();
            entity.Property(p => p.ProductName).HasMaxLength(40).IsRequired();
            entity.Property(p => p.CategoryId).HasMaxLength(50);
            entity.Property(p => p.SupplierId).HasMaxLength(50);
            entity.Property(p => p.QuantityPerUnit).HasMaxLength(20);
            entity.Property(p => p.UnitPrice).HasColumnType("money");

            // Configure relationships
            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(p => p.Supplier)
                  .WithMany(s => s.Products)
                  .HasForeignKey(p => p.SupplierId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.CategoryId);
            entity.Property(c => c.CategoryId).HasMaxLength(50).IsRequired();
            entity.Property(c => c.CategoryName).HasMaxLength(15).IsRequired();
            entity.Property(c => c.Description).HasColumnType("ntext");
            entity.Property(c => c.PictureLink).HasMaxLength(255);
        });

        // Configure Supplier entity
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(s => s.SupplierId);
            entity.Property(s => s.SupplierId).HasMaxLength(50).IsRequired();
            entity.Property(s => s.CompanyName).HasMaxLength(40).IsRequired();
            entity.Property(s => s.ContactName).HasMaxLength(30);
            entity.Property(s => s.ContactTitle).HasMaxLength(30);
            entity.Property(s => s.Address).HasMaxLength(60);
            entity.Property(s => s.City).HasMaxLength(15);
            entity.Property(s => s.Region).HasMaxLength(15);
            entity.Property(s => s.PostalCode).HasMaxLength(10);
            entity.Property(s => s.Country).HasMaxLength(15);
            entity.Property(s => s.Phone).HasMaxLength(24);
            entity.Property(s => s.Fax).HasMaxLength(24);
            entity.Property(s => s.HomePage).HasColumnType("ntext");
        });
    }
}
