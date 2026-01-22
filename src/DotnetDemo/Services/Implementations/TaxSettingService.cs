using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 稅別設定服務實作
/// </summary>
public class TaxSettingService : ITaxSettingService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public TaxSettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<TaxSetting>> GetAllAsync()
    {
        return await _context.TaxSettings
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<TaxSetting>> GetActiveAsync()
    {
        return await _context.TaxSettings
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TaxSetting?> GetByIdAsync(int id)
    {
        return await _context.TaxSettings.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<TaxSetting?> GetDefaultAsync()
    {
        return await _context.TaxSettings
            .FirstOrDefaultAsync(x => x.IsDefault && x.IsActive);
    }

    /// <inheritdoc/>
    public async Task<TaxSetting> CreateAsync(TaxSetting taxSetting)
    {
        if (taxSetting.IsDefault)
        {
            await ClearDefaultAsync();
        }

        _context.TaxSettings.Add(taxSetting);
        await _context.SaveChangesAsync();
        return taxSetting;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(TaxSetting taxSetting)
    {
        var existing = await _context.TaxSettings.FindAsync(taxSetting.Id);
        if (existing == null) return false;

        if (taxSetting.IsDefault && !existing.IsDefault)
        {
            await ClearDefaultAsync();
        }

        existing.Name = taxSetting.Name;
        existing.Code = taxSetting.Code;
        existing.Rate = taxSetting.Rate;
        existing.IsDefault = taxSetting.IsDefault;
        existing.IsInclusive = taxSetting.IsInclusive;
        existing.Description = taxSetting.Description;
        existing.IsActive = taxSetting.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var taxSetting = await _context.TaxSettings.FindAsync(id);
        if (taxSetting == null)
            return (false, "稅別不存在");

        if (taxSetting.IsDefault)
            return (false, "無法刪除預設稅別");

        _context.TaxSettings.Remove(taxSetting);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    private async Task ClearDefaultAsync()
    {
        var currentDefault = await _context.TaxSettings
            .Where(x => x.IsDefault)
            .ToListAsync();

        foreach (var tax in currentDefault)
        {
            tax.IsDefault = false;
        }
    }
}
