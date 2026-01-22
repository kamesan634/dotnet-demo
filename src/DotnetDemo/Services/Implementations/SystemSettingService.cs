using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 系統參數服務實作
/// </summary>
public class SystemSettingService : ISystemSettingService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public SystemSettingService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<SystemSetting>> GetAllAsync()
    {
        return await _context.SystemSettings
            .OrderBy(x => x.Group)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Key)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<SystemSetting>> GetByGroupAsync(string group)
    {
        return await _context.SystemSettings
            .Where(x => x.Group == group)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Key)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<SystemSetting?> GetByKeyAsync(string key)
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(x => x.Key == key);
    }

    /// <inheritdoc/>
    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await GetByKeyAsync(key);
        return setting?.Value;
    }

    /// <inheritdoc/>
    public async Task<int> GetIntValueAsync(string key, int defaultValue = 0)
    {
        var value = await GetValueAsync(key);
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <inheritdoc/>
    public async Task<bool> GetBoolValueAsync(string key, bool defaultValue = false)
    {
        var value = await GetValueAsync(key);
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <inheritdoc/>
    public async Task<bool> SetValueAsync(string key, string value, int? userId = null)
    {
        var setting = await GetByKeyAsync(key);
        if (setting == null) return false;

        setting.Value = value;
        setting.UpdatedAt = DateTime.UtcNow;
        setting.UpdatedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<SystemSetting> CreateAsync(SystemSetting setting)
    {
        _context.SystemSettings.Add(setting);
        await _context.SaveChangesAsync();
        return setting;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(SystemSetting setting, int? userId = null)
    {
        var existing = await _context.SystemSettings.FindAsync(setting.Id);
        if (existing == null) return false;

        if (!existing.IsEditable)
            return false;

        existing.Value = setting.Value;
        existing.Description = setting.Description;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var setting = await _context.SystemSettings.FindAsync(id);
        if (setting == null)
            return (false, "參數不存在");

        if (!setting.IsEditable)
            return (false, "此參數無法刪除");

        _context.SystemSettings.Remove(setting);
        await _context.SaveChangesAsync();
        return (true, null);
    }
}
