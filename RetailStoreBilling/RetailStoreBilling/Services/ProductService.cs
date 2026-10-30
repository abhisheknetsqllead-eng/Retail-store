using Microsoft.EntityFrameworkCore;
using RetailStoreBilling.Data;
using RetailStoreBilling.Models;

namespace RetailStoreBilling.Services;

public class ProductService(IDbContextFactory<RetailDbContext> dbFactory)
{
    public async Task<List<Product>> GetAllAsync(string? search = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var query = db.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));
        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Products.FindAsync(id);
    }

    public async Task SaveAsync(Product product)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        if (product.Id == 0)
            db.Products.Add(product);
        else
            db.Products.Update(product);
        await db.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var product = await db.Products.FindAsync(id);
        if (product is null) return false;
        var used = await db.SaleItems.AnyAsync(i => i.ProductId == id);
        if (used) return false;
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }
}
