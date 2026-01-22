using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 商品組合服務實作
/// </summary>
public class ProductBundleService : IProductBundleService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public ProductBundleService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<ProductBundle>> SearchAsync(string? keyword, bool? isActive)
    {
        var query = _context.ProductBundles
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.Code.Contains(keyword));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        return await query.OrderBy(x => x.Name).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ProductBundle>> GetAllAsync()
    {
        return await _context.ProductBundles
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ProductBundle>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.ProductBundles
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .Where(x => x.IsActive)
            .Where(x => x.StartDate == null || x.StartDate <= now)
            .Where(x => x.EndDate == null || x.EndDate >= now)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ProductBundle?> GetByIdAsync(int id)
    {
        return await _context.ProductBundles
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<ProductBundle> CreateAsync(ProductBundle bundle)
    {
        bundle.OriginalTotal = await CalculateOriginalTotalAsync(bundle.Items.ToList());
        _context.ProductBundles.Add(bundle);
        await _context.SaveChangesAsync();
        return bundle;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(ProductBundle bundle)
    {
        var existing = await _context.ProductBundles
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == bundle.Id);

        if (existing == null) return false;

        existing.Code = bundle.Code;
        existing.Name = bundle.Name;
        existing.Description = bundle.Description;
        existing.SellingPrice = bundle.SellingPrice;
        existing.StartDate = bundle.StartDate;
        existing.EndDate = bundle.EndDate;
        existing.IsActive = bundle.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.ProductBundleItems.RemoveRange(existing.Items);
        foreach (var item in bundle.Items)
        {
            item.ProductBundleId = existing.Id;
            _context.ProductBundleItems.Add(item);
        }

        existing.OriginalTotal = await CalculateOriginalTotalAsync(bundle.Items.ToList());

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var bundle = await _context.ProductBundles
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (bundle == null)
            return (false, "商品組合不存在");

        _context.ProductBundleItems.RemoveRange(bundle.Items);
        _context.ProductBundles.Remove(bundle);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    /// <inheritdoc/>
    public async Task<decimal> CalculateOriginalTotalAsync(List<ProductBundleItem> items)
    {
        decimal total = 0;
        foreach (var item in items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                total += product.SellingPrice * item.Quantity;
            }
        }
        return total;
    }
}
