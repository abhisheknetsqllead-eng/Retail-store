using Microsoft.EntityFrameworkCore;
using RetailStoreBilling.Models;

namespace RetailStoreBilling.Data;

public class RetailDbContext(DbContextOptions<RetailDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();
        modelBuilder.Entity<Product>().Property(p => p.Sku).HasMaxLength(40).IsRequired();
        modelBuilder.Entity<Product>().HasIndex(p => p.Sku).IsUnique();

        modelBuilder.Entity<Sale>().Property(s => s.Subtotal).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(s => s.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(s => s.Total).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(s => s.CustomerName).HasMaxLength(120);
        modelBuilder.Entity<SaleItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<SaleItem>().Property(i => i.LineTotal).HasPrecision(18, 2);

        modelBuilder.Entity<SaleItem>()
            .HasOne(i => i.Sale).WithMany(s => s.Items)
            .HasForeignKey(i => i.SaleId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SaleItem>()
            .HasOne(i => i.Product).WithMany()
            .HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
