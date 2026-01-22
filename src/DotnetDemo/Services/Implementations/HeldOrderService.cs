using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 掛單服務實作
/// </summary>
public class HeldOrderService : IHeldOrderService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public HeldOrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<HeldOrder>> GetByStoreAsync(int storeId)
    {
        return await _context.HeldOrders
            .Include(x => x.Store)
            .Include(x => x.Customer)
            .Where(x => x.StoreId == storeId)
            .Where(x => x.ExpiresAt == null || x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<HeldOrder?> GetByIdAsync(int id)
    {
        return await _context.HeldOrders
            .Include(x => x.Store)
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<HeldOrder> CreateAsync(HeldOrder heldOrder)
    {
        if (string.IsNullOrEmpty(heldOrder.HoldNumber))
        {
            heldOrder.HoldNumber = await GenerateHoldNumberAsync();
        }

        if (heldOrder.ExpiresAt == null)
        {
            heldOrder.ExpiresAt = DateTime.UtcNow.AddHours(24);
        }

        _context.HeldOrders.Add(heldOrder);
        await _context.SaveChangesAsync();
        return heldOrder;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var heldOrder = await _context.HeldOrders.FindAsync(id);
        if (heldOrder == null) return false;

        _context.HeldOrders.Remove(heldOrder);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<int> CleanupExpiredAsync()
    {
        var expired = await _context.HeldOrders
            .Where(x => x.ExpiresAt != null && x.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        if (expired.Any())
        {
            _context.HeldOrders.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }

        return expired.Count;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateHoldNumberAsync()
    {
        var today = DateTime.Today;
        var count = await _context.HeldOrders
            .CountAsync(x => x.CreatedAt >= today);

        return $"HOLD{DateTime.Now:yyyyMMdd}{(count + 1).ToString().PadLeft(4, '0')}";
    }
}
