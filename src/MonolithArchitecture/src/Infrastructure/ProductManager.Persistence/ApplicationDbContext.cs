using System.Data;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
namespace ProductManager.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork, IDataProtectionKeyContext
{
    private IDbContextTransaction _dbContextTransaction = null!;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AlphabeticalListOfProduct> AlphabeticalListOfProducts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategorySalesFor1997> CategorySalesFor1997S { get; set; }

    public virtual DbSet<CurrentProductList> CurrentProductLists { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAndSuppliersByCity> CustomerAndSuppliersByCities { get; set; }

    public virtual DbSet<CustomerDemographic> CustomerDemographics { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailsExtended> OrderDetailsExtendeds { get; set; }

    public virtual DbSet<OrderSubtotal> OrderSubtotals { get; set; }

    public virtual DbSet<OrdersQry> OrdersQueries { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductSalesFor1997> ProductSalesFor1997S { get; set; }

    public virtual DbSet<ProductsAboveAveragePrice> ProductsAboveAveragePrices { get; set; }

    public virtual DbSet<ProductsByCategory> ProductsByCategories { get; set; }

    public virtual DbSet<QuarterlyOrder> QuarterlyOrders { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<SalesByCategory> SalesByCategories { get; set; }

    public virtual DbSet<SalesTotalsByAmount> SalesTotalsByAmounts { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<SummaryOfSalesByQuarter> SummaryOfSalesByQuarters { get; set; }

    public virtual DbSet<SummaryOfSalesByYear> SummaryOfSalesByYears { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<Territory> Territories { get; set; }
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        _dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _dbContextTransaction;
    }
    public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        String? lockName = null,
        CancellationToken cancellationToken = default)
    {
        _dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _dbContextTransaction;
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        => await _dbContextTransaction.CommitAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlphabeticalListOfProduct>(entity =>
        {
            entity.ToView("Alphabetical list of products");
        });

        modelBuilder.Entity<CategorySalesFor1997>(entity =>
        {
            entity.ToView("Category Sales for 1997");
        });

        modelBuilder.Entity<CurrentProductList>(entity =>
        {
            entity.ToView("Current Product List");

            entity.Property(e => e.ProductId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).IsFixedLength();

            entity.HasMany(d => d.CustomerTypes).WithMany(p => p.Customers)
                .UsingEntity<Dictionary<string, object>>(
                "CustomerCustomerDemo",
                configureRight: r => r.HasOne<CustomerDemographic>().WithMany()
                    .HasForeignKey("CustomerTypeId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerCustomerDemo"),
                configureLeft: l => l.HasOne<Customer>().WithMany()
                    .HasForeignKey("CustomerId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerCustomerDemo_Customers"),
                configureJoinEntityType: j =>
                {
                    j.HasKey("CustomerId", "CustomerTypeId").IsClustered(false);
                    j.ToTable("CustomerCustomerDemo");
                    j.IndexerProperty<string>("CustomerId")
                        .HasMaxLength(5)
                        .IsFixedLength()
                        .HasColumnName("CustomerID");
                    j.IndexerProperty<string>("CustomerTypeId")
                        .HasMaxLength(10)
                        .IsFixedLength()
                        .HasColumnName("CustomerTypeID");
                });
        });

        modelBuilder.Entity<CustomerAndSuppliersByCity>(entity =>
        {
            entity.ToView("Customer and Suppliers by City");
        });

        modelBuilder.Entity<CustomerDemographic>(entity =>
        {
            entity.HasKey(e => e.CustomerTypeId).IsClustered(false);

            entity.Property(e => e.CustomerTypeId).IsFixedLength();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasOne(d => d.ReportsToNavigation).WithMany(p => p.InverseReportsToNavigation)
                .HasConstraintName("FK_Employees_Employees");

            entity.HasMany(d => d.Territories).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                "EmployeeTerritory",
                configureRight: r => r.HasOne<Territory>().WithMany()
                    .HasForeignKey("TerritoryId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTerritories_Territories"),
                configureLeft: l => l.HasOne<Employee>().WithMany()
                    .HasForeignKey("EmployeeId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeTerritories_Employees"),
                configureJoinEntityType: j =>
                {
                    j.HasKey("EmployeeId", "TerritoryId").IsClustered(false);
                    j.ToTable("EmployeeTerritories");
                    j.IndexerProperty<int>("EmployeeId").HasColumnName("EmployeeID");
                    j.IndexerProperty<string>("TerritoryId")
                        .HasMaxLength(20)
                        .HasColumnName("TerritoryID");
                });
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToView("Invoices");

            entity.Property(e => e.CustomerId).IsFixedLength();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.CustomerId).IsFixedLength();
            entity.Property(e => e.Freight).HasDefaultValue(0m);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Employee).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Employees");

            entity.HasOne(d => d.ShipViaNavigation).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Shippers");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new
            {
                e.OrderId, e.ProductId
            }).HasName("PK_Order_Details");

            entity.Property(e => e.Quantity).HasDefaultValue((short)1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Details_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_Details_Products");
        });

        modelBuilder.Entity<OrderDetailsExtended>(entity =>
        {
            entity.ToView("Order Details Extended");
        });

        modelBuilder.Entity<OrderSubtotal>(entity =>
        {
            entity.ToView("Order Subtotals");
        });

        modelBuilder.Entity<OrdersQry>(entity =>
        {
            entity.ToView("Orders Qry");

            entity.Property(e => e.CustomerId).IsFixedLength();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ReorderLevel).HasDefaultValue((short)0);
            entity.Property(e => e.UnitPrice).HasDefaultValue(0m);
            entity.Property(e => e.UnitsInStock).HasDefaultValue((short)0);
            entity.Property(e => e.UnitsOnOrder).HasDefaultValue((short)0);

            entity.HasOne(d => d.Category).WithMany(p => p.Products).HasConstraintName("FK_Products_Categories");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Products).HasConstraintName("FK_Products_Suppliers");
        });

        modelBuilder.Entity<ProductSalesFor1997>(entity =>
        {
            entity.ToView("Product Sales for 1997");
        });

        modelBuilder.Entity<ProductsAboveAveragePrice>(entity =>
        {
            entity.ToView("Products Above Average Price");
        });

        modelBuilder.Entity<ProductsByCategory>(entity =>
        {
            entity.ToView("Products by Category");
        });

        modelBuilder.Entity<QuarterlyOrder>(entity =>
        {
            entity.ToView("Quarterly Orders");

            entity.Property(e => e.CustomerId).IsFixedLength();
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).IsClustered(false);

            entity.Property(e => e.RegionId).ValueGeneratedNever();
            entity.Property(e => e.RegionDescription).IsFixedLength();
        });

        modelBuilder.Entity<SalesByCategory>(entity =>
        {
            entity.ToView("Sales by Category");
        });

        modelBuilder.Entity<SalesTotalsByAmount>(entity =>
        {
            entity.ToView("Sales Totals by Amount");
        });

        modelBuilder.Entity<SummaryOfSalesByQuarter>(entity =>
        {
            entity.ToView("Summary of Sales by Quarter");
        });

        modelBuilder.Entity<SummaryOfSalesByYear>(entity =>
        {
            entity.ToView("Summary of Sales by Year");
        });

        modelBuilder.Entity<Territory>(entity =>
        {
            entity.HasKey(e => e.TerritoryId).IsClustered(false);

            entity.Property(e => e.TerritoryDescription).IsFixedLength();

            entity.HasOne(d => d.Region).WithMany(p => p.Territories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Territories_Region");
        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.EnableSensitiveDataLogging(false);
}
