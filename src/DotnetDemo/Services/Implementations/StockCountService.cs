using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 庫存盤點服務實作
/// </summary>
public class StockCountService : IStockCountService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public StockCountService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<StockCount>> SearchAsync(int? warehouseId, StockCountStatus? status, DateTime? from, DateTime? to)
    {
        var query = _context.StockCounts
            .Include(x => x.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == warehouseId.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (from.HasValue)
            query = query.Where(x => x.CountDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CountDate <= to.Value);

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<StockCount?> GetByIdAsync(int id)
    {
        return await _context.StockCounts
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<StockCount> CreateAsync(StockCount stockCount, int userId)
    {
        stockCount.CountNumber = await GenerateCountNumberAsync();
        stockCount.CreatedBy = userId;
        stockCount.Status = StockCountStatus.Draft;

        _context.StockCounts.Add(stockCount);
        await _context.SaveChangesAsync();

        return stockCount;
    }

    /// <inheritdoc/>
    public async Task<bool> InitializeItemsAsync(int stockCountId)
    {
        var stockCount = await _context.StockCounts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == stockCountId);

        if (stockCount == null || stockCount.Status != StockCountStatus.Draft)
            return false;

        var inventories = await _context.Inventories
            .Include(x => x.Product)
            .Where(x => x.WarehouseId == stockCount.WarehouseId && x.Quantity > 0)
            .ToListAsync();

        foreach (var inv in inventories)
        {
            var item = new StockCountItem
            {
                StockCountId = stockCountId,
                ProductId = inv.ProductId,
                SystemQuantity = inv.Quantity,
                IsCounted = false
            };
            _context.StockCountItems.Add(item);
        }

        stockCount.Status = StockCountStatus.InProgress;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateItemQuantityAsync(int itemId, int actualQuantity, int userId, string? reason = null)
    {
        var item = await _context.StockCountItems.FindAsync(itemId);
        if (item == null) return false;

        item.ActualQuantity = actualQuantity;
        item.Difference = actualQuantity - item.SystemQuantity;
        item.DifferenceReason = reason;
        item.IsCounted = true;
        item.CountedAt = DateTime.UtcNow;
        item.CountedBy = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> CompleteAsync(int id, int userId)
    {
        var stockCount = await _context.StockCounts
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (stockCount == null)
            return (false, "盤點單不存在");

        if (stockCount.Status != StockCountStatus.InProgress)
            return (false, "盤點單狀態不正確");

        var uncountedItems = stockCount.Items.Where(x => !x.IsCounted).ToList();
        if (uncountedItems.Any())
            return (false, $"尚有 {uncountedItems.Count} 項商品未盤點");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in stockCount.Items.Where(x => x.Difference != 0))
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseId == stockCount.WarehouseId);

                if (inventory != null && item.ActualQuantity.HasValue)
                {
                    var beforeQuantity = inventory.Quantity;
                    var movement = new InventoryMovement
                    {
                        ProductId = item.ProductId,
                        WarehouseId = stockCount.WarehouseId,
                        Type = item.Difference > 0 ? MovementType.Adjustment : MovementType.Adjustment,
                        Quantity = Math.Abs(item.Difference.Value),
                        BeforeQuantity = beforeQuantity,
                        AfterQuantity = item.ActualQuantity.Value,
                        ReferenceType = "StockCount",
                        ReferenceNumber = stockCount.CountNumber,
                        Notes = $"盤點調整：{item.DifferenceReason}",
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.InventoryMovements.Add(movement);

                    inventory.Quantity = item.ActualQuantity.Value;
                    inventory.UpdatedAt = DateTime.UtcNow;
                }
            }

            stockCount.TotalSurplus = stockCount.Items.Where(x => x.Difference > 0).Sum(x => x.Difference ?? 0);
            stockCount.TotalShortage = Math.Abs(stockCount.Items.Where(x => x.Difference < 0).Sum(x => x.Difference ?? 0));
            stockCount.Status = StockCountStatus.Completed;
            stockCount.CompletedAt = DateTime.UtcNow;
            stockCount.CompletedBy = userId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"完成盤點失敗：{ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CancelAsync(int id, int userId)
    {
        var stockCount = await _context.StockCounts.FindAsync(id);
        if (stockCount == null) return false;

        if (stockCount.Status == StockCountStatus.Completed)
            return false;

        stockCount.Status = StockCountStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateCountNumberAsync()
    {
        var today = DateTime.Today;
        var count = await _context.StockCounts
            .CountAsync(x => x.CreatedAt >= today);

        return $"SC{DateTime.Now:yyyyMMdd}{(count + 1).ToString().PadLeft(4, '0')}";
    }
}
