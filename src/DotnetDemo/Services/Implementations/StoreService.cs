using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 門市服務實作
/// </summary>
public class StoreService : IStoreService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StoreService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public StoreService(ApplicationDbContext context, ILogger<StoreService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Store>> GetAllAsync()
    {
        return await _context.Stores
            .OrderBy(s => s.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Store>> GetActiveAsync()
    {
        return await _context.Stores
            .Where(s => s.IsActive)
            .OrderBy(s => s.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Store?> GetByIdAsync(int id)
    {
        return await _context.Stores.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Store> CreateAsync(Store store)
    {
        store.CreatedAt = DateTime.UtcNow;
        _context.Stores.Add(store);
        await _context.SaveChangesAsync();

        _logger.LogInformation("門市 {Code} 已建立", store.Code);
        return store;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Store store)
    {
        var existing = await _context.Stores.FindAsync(store.Id);
        if (existing == null) return false;

        existing.Name = store.Name;
        existing.Address = store.Address;
        existing.Phone = store.Phone;
        existing.IsPrimary = store.IsPrimary;
        existing.IsActive = store.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("門市 {Code} 已更新", store.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return false;

        _context.Stores.Remove(store);
        await _context.SaveChangesAsync();

        _logger.LogInformation("門市 {Code} 已刪除", store.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.Stores.Where(s => s.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }
}
