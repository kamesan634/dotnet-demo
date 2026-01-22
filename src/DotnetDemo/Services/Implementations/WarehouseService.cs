using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 倉庫服務實作
/// </summary>
public class WarehouseService : IWarehouseService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WarehouseService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public WarehouseService(ApplicationDbContext context, ILogger<WarehouseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Warehouse>> GetAllAsync()
    {
        return await _context.Warehouses
            .Include(w => w.Store)
            .OrderBy(w => w.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Warehouse>> GetActiveAsync()
    {
        return await _context.Warehouses
            .Include(w => w.Store)
            .Where(w => w.IsActive)
            .OrderBy(w => w.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Warehouse?> GetByIdAsync(int id)
    {
        return await _context.Warehouses
            .Include(w => w.Store)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    /// <inheritdoc />
    public async Task<Warehouse> CreateAsync(Warehouse warehouse)
    {
        warehouse.CreatedAt = DateTime.UtcNow;
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        _logger.LogInformation("倉庫 {Code} 已建立", warehouse.Code);
        return warehouse;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Warehouse warehouse)
    {
        var existing = await _context.Warehouses.FindAsync(warehouse.Id);
        if (existing == null) return false;

        existing.Name = warehouse.Name;
        existing.StoreId = warehouse.StoreId;
        existing.Address = warehouse.Address;
        existing.ContactName = warehouse.ContactName;
        existing.Phone = warehouse.Phone;
        existing.IsPrimary = warehouse.IsPrimary;
        existing.IsActive = warehouse.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("倉庫 {Code} 已更新", warehouse.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null) return false;

        // 檢查是否有庫存記錄
        var hasInventory = await _context.Inventories.AnyAsync(i => i.WarehouseId == id);
        if (hasInventory)
        {
            _logger.LogWarning("倉庫 {Code} 有庫存記錄，無法刪除", warehouse.Code);
            return false;
        }

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();

        _logger.LogInformation("倉庫 {Code} 已刪除", warehouse.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.Warehouses.Where(w => w.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(w => w.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }

    /// <inheritdoc />
    public async Task<List<Warehouse>> GetByStoreAsync(int storeId)
    {
        return await _context.Warehouses
            .Where(w => w.StoreId == storeId && w.IsActive)
            .OrderBy(w => w.Code)
            .ToListAsync();
    }
}
