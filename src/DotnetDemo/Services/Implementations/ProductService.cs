using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 商品服務實作
/// </summary>
public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .OrderBy(p => p.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Product>> SearchAsync(string? searchText, int? categoryId, bool? isActive)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(p =>
                p.Name.Contains(searchText) ||
                p.Code.Contains(searchText) ||
                (p.Barcode != null && p.Barcode.Contains(searchText)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        return await query.OrderBy(p => p.Code).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .Include(p => p.DefaultSupplier)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("商品 {Code} 已建立", product.Code);
        return product;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Product product)
    {
        var existing = await _context.Products.FindAsync(product.Id);
        if (existing == null) return false;

        existing.Name = product.Name;
        existing.Barcode = product.Barcode;
        existing.Description = product.Description;
        existing.CategoryId = product.CategoryId;
        existing.UnitId = product.UnitId;
        existing.CostPrice = product.CostPrice;
        existing.SellingPrice = product.SellingPrice;
        existing.SafetyStock = product.SafetyStock;
        existing.DefaultSupplierId = product.DefaultSupplierId;
        existing.IsActive = product.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("商品 {Code} 已更新", product.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("商品 {Code} 已刪除", product.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.Products.Where(p => p.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    /// <inheritdoc />
    public async Task<List<Product>> SearchForPosAsync(string searchText, int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return new List<Product>();

        return await _context.Products
            .Where(p => p.IsActive &&
                (p.Name.Contains(searchText) ||
                (p.Barcode != null && p.Barcode.Contains(searchText))))
            .Take(limit)
            .ToListAsync();
    }
}
