using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 會員等級服務實作
/// </summary>
public class CustomerLevelService : ICustomerLevelService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomerLevelService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public CustomerLevelService(ApplicationDbContext context, ILogger<CustomerLevelService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<CustomerLevel>> GetAllAsync()
    {
        return await _context.CustomerLevels
            .OrderBy(l => l.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<CustomerLevel>> GetActiveAsync()
    {
        return await _context.CustomerLevels
            .Where(l => l.IsActive)
            .OrderBy(l => l.SortOrder)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<CustomerLevel?> GetByIdAsync(int id)
    {
        return await _context.CustomerLevels.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<CustomerLevel> CreateAsync(CustomerLevel level)
    {
        _context.CustomerLevels.Add(level);
        await _context.SaveChangesAsync();

        _logger.LogInformation("會員等級 {Name} 已建立", level.Name);
        return level;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(CustomerLevel level)
    {
        var existing = await _context.CustomerLevels.FindAsync(level.Id);
        if (existing == null) return false;

        existing.Name = level.Name;
        existing.RequiredAmount = level.RequiredAmount;
        existing.DiscountPercent = level.DiscountPercent;
        existing.PointMultiplier = level.PointMultiplier;
        existing.SortOrder = level.SortOrder;
        existing.IsActive = level.IsActive;

        await _context.SaveChangesAsync();

        _logger.LogInformation("會員等級 {Name} 已更新", level.Name);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var level = await _context.CustomerLevels.FindAsync(id);
        if (level == null) return false;

        // 檢查是否有客戶使用此等級
        var hasCustomers = await _context.Customers.AnyAsync(c => c.CustomerLevelId == id);
        if (hasCustomers)
        {
            _logger.LogWarning("會員等級 {Name} 有客戶使用中，無法刪除", level.Name);
            return false;
        }

        _context.CustomerLevels.Remove(level);
        await _context.SaveChangesAsync();

        _logger.LogInformation("會員等級 {Name} 已刪除", level.Name);
        return true;
    }

    /// <inheritdoc />
    public async Task<CustomerLevel?> GetLevelByAmountAsync(decimal amount)
    {
        return await _context.CustomerLevels
            .Where(l => l.IsActive && l.RequiredAmount <= amount)
            .OrderByDescending(l => l.RequiredAmount)
            .FirstOrDefaultAsync();
    }
}
