using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 分類服務實作
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .OrderBy(c => c.Level)
            .ThenBy(c => c.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Category>> GetActiveAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Level)
            .ThenBy(c => c.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc />
    public async Task<Category> CreateAsync(Category category)
    {
        category.Level = category.ParentId.HasValue ? 2 : 1;
        category.CreatedAt = DateTime.UtcNow;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation("分類 {Code} 已建立", category.Code);
        return category;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Category category)
    {
        var existing = await _context.Categories.FindAsync(category.Id);
        if (existing == null) return false;

        existing.Name = category.Name;
        existing.ParentId = category.ParentId;
        existing.Level = category.ParentId.HasValue ? 2 : 1;
        existing.SortOrder = category.SortOrder;
        existing.IsActive = category.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("分類 {Code} 已更新", category.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        if (!await CanDeleteAsync(id)) return false;

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        _logger.LogInformation("分類 {Code} 已刪除", category.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CanDeleteAsync(int id)
    {
        var hasChildren = await _context.Categories.AnyAsync(c => c.ParentId == id);
        if (hasChildren) return false;

        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
        if (hasProducts) return false;

        return true;
    }
}
