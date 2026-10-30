using Microsoft.EntityFrameworkCore;
using RetailStoreBilling.Data;
using RetailStoreBilling.Models;

namespace RetailStoreBilling.Services;

public record CartLine(int ProductId, int Quantity);

public class BillingService(IDbContextFactory<RetailDbContext> dbFactory)
{
    public async Task<List<Sale>> GetSalesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Sales.AsNoTracking().Include(s => s.Items)
            .OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<Sale?> GetSaleAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Sales.AsNoTracking().Include(s => s.Items)
            .ThenInclude(i => i.Product).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sale> CreateSaleAsync(string? customerName, string paymentMethod,
        decimal taxRatePercent, IReadOnlyCollection<CartLine> lines)
    {
        if (lines.Count == 0) throw new InvalidOperationException("Add at least one item to the bill.");
        if (taxRatePercent < 0 || taxRatePercent > 100)
            throw new InvalidOperationException("Tax rate must be between 0 and 100.");

        await using var db = await dbFactory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        var sale = new Sale
        {
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? null : customerName.Trim(),
            PaymentMethod = paymentMethod,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var line in lines)
        {
            if (line.Quantity < 1) throw new InvalidOperationException("Quantity must be at least one.");
            var product = await db.Products.SingleOrDefaultAsync(p => p.Id == line.ProductId && p.IsActive);
            if (product is null) throw new InvalidOperationException($"Product {line.ProductId} is unavailable.");
            if (product.StockQuantity < line.Quantity)
                throw new InvalidOperationException($"Not enough stock for {product.Name}. Available: {product.StockQuantity}.");

            product.StockQuantity -= line.Quantity;
            sale.Items.Add(new SaleItem
            {
                ProductId = product.Id,
                Quantity = line.Quantity,
                UnitPrice = product.Price,
                LineTotal = product.Price * line.Quantity
            });
        }

        sale.Subtotal = sale.Items.Sum(i => i.LineTotal);
        sale.TaxAmount = Math.Round(sale.Subtotal * taxRatePercent / 100m, 2, MidpointRounding.AwayFromZero);
        sale.Total = sale.Subtotal + sale.TaxAmount;
        db.Sales.Add(sale);
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return sale;
    }

    public async Task<bool> DeleteSaleAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var sale = await db.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == id);
        if (sale is null) return false;

        // Reversing a sale restores inventory. In a production system, consider void/refund audit records.
        foreach (var item in sale.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId);
            if (product is not null) product.StockQuantity += item.Quantity;
        }
        db.Sales.Remove(sale);
        await db.SaveChangesAsync();
        return true;
    }
}
