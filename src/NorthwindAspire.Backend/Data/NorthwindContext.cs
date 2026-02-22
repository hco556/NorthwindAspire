using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Data;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeTerritory> EmployeeTerritories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Shipper> Shippers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Territory> Territories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure primary keys
        modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<Employee>().HasKey(e => e.EmployeeId);
        modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
        modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.OrderId, od.ProductId });
        modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
        modelBuilder.Entity<Region>().HasKey(r => r.RegionId);
        modelBuilder.Entity<Shipper>().HasKey(s => s.ShipperId);
        modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);
        modelBuilder.Entity<Territory>().HasKey(t => t.TerritoryId);
        modelBuilder.Entity<EmployeeTerritory>().HasKey(et => new { et.EmployeeId, et.TerritoryId });

        // Configure foreign keys and relationships
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Employee)
            .WithMany(e => e.Orders)
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Shipper)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.ShipVia)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Manager)
            .WithMany(e => e.Subordinates)
            .HasForeignKey(e => e.ReportsTo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeTerritory>()
            .HasOne(et => et.Employee)
            .WithMany(e => e.Territories)
            .HasForeignKey(et => et.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EmployeeTerritory>()
            .HasOne(et => et.Territory)
            .WithMany(t => t.Employees)
            .HasForeignKey(et => et.TerritoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Territory>()
            .HasOne(t => t.Region)
            .WithMany(r => r.Territories)
            .HasForeignKey(t => t.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
