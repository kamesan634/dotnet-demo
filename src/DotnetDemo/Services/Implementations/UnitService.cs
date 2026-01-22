using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 單位服務實作
/// </summary>
public class UnitService : IUnitService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UnitService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public UnitService(ApplicationDbContext context, ILogger<UnitService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Unit>> GetAllAsync()
    {
        return await _context.Units
            .OrderBy(u => u.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Unit>> GetActiveAsync()
    {
        return await _context.Units
            .Where(u => u.IsActive)
            .OrderBy(u => u.Code)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Unit?> GetByIdAsync(int id)
    {
        return await _context.Units.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<Unit> CreateAsync(Unit unit)
    {
        unit.CreatedAt = DateTime.UtcNow;
        _context.Units.Add(unit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("單位 {Code} 已建立", unit.Code);
        return unit;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Unit unit)
    {
        var existing = await _context.Units.FindAsync(unit.Id);
        if (existing == null) return false;

        existing.Name = unit.Name;
        existing.IsActive = unit.IsActive;

        await _context.SaveChangesAsync();

        _logger.LogInformation("單位 {Code} 已更新", unit.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit == null) return false;

        // 檢查是否有商品使用此單位
        var hasProducts = await _context.Products.AnyAsync(p => p.UnitId == id);
        if (hasProducts)
        {
            _logger.LogWarning("單位 {Code} 有商品使用中，無法刪除", unit.Code);
            return false;
        }

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("單位 {Code} 已刪除", unit.Code);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.Units.Where(u => u.Code == code);
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        return await query.AnyAsync();
    }
}
