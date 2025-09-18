using Microsoft.EntityFrameworkCore;
using CustomerManagement.Domain.Entities;

namespace CustomerManagement.Infrastructure.Data;

/// <summary>
/// Customer management database context
/// </summary>
public class CustomerManagementDbContext : DbContext
{
  public CustomerManagementDbContext(DbContextOptions<CustomerManagementDbContext> options) : base(options)
  {
  }

  public DbSet<Customer> Customers { get; set; } = null!;
  public DbSet<Order> Orders { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configure Customer entity
    modelBuilder.Entity<Customer>(entity =>
    {
      entity.ToTable("Customers");
      entity.HasKey(c => c.Id);
      entity.Property(c => c.Id).HasMaxLength(50).IsRequired();
      entity.Property(c => c.CompanyName).HasMaxLength(40).IsRequired();
      entity.Property(c => c.ContactName).HasMaxLength(30);
      entity.Property(c => c.ContactTitle).HasMaxLength(30);
      entity.Property(c => c.Address).HasMaxLength(60);
      entity.Property(c => c.City).HasMaxLength(15);
      entity.Property(c => c.Region).HasMaxLength(15);
      entity.Property(c => c.PostalCode).HasMaxLength(10);
      entity.Property(c => c.Country).HasMaxLength(15);
      entity.Property(c => c.Phone).HasMaxLength(24);
      entity.Property(c => c.Fax).HasMaxLength(24);
      entity.Property(c => c.Email).HasMaxLength(50);
    });

    // Configure Order entity
    modelBuilder.Entity<Order>(entity =>
    {
      entity.ToTable("Orders");
      entity.HasKey(o => o.Id);
      entity.Property(o => o.Id).ValueGeneratedOnAdd();
      entity.Property(o => o.CustomerId).HasMaxLength(5).IsRequired();
      entity.Property(o => o.ShipName).HasMaxLength(40);
      entity.Property(o => o.ShipAddress).HasMaxLength(60);
      entity.Property(o => o.ShipCity).HasMaxLength(15);
      entity.Property(o => o.ShipRegion).HasMaxLength(15);
      entity.Property(o => o.ShipPostalCode).HasMaxLength(10);
      entity.Property(o => o.ShipCountry).HasMaxLength(15);
      entity.Property(o => o.Freight).HasColumnType("money");

      // Configure relationships
      entity.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
    });
  }
}
