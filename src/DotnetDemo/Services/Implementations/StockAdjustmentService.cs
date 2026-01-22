using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 庫存調整服務實作
/// </summary>
public class StockAdjustmentService : IStockAdjustmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StockAdjustmentService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public StockAdjustmentService(ApplicationDbContext context, ILogger<StockAdjustmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<StockAdjustment>> SearchAsync(string? adjustmentNumber, int? warehouseId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StockAdjustments
            .Include(a => a.Warehouse)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(adjustmentNumber))
        {
            query = query.Where(a => a.AdjustmentNumber.Contains(adjustmentNumber));
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(a => a.WarehouseId == warehouseId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value.AddDays(1);
            query = query.Where(a => a.CreatedAt < end);
        }

        return await query.OrderByDescending(a => a.CreatedAt).Take(100).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<StockAdjustment?> GetByIdAsync(int id)
    {
        return await _context.StockAdjustments
            .Include(a => a.Warehouse)
            .Include(a => a.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <inheritdoc />
    public async Task<StockAdjustment> CreateAsync(StockAdjustment adjustment, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 產生調整單號
            adjustment.AdjustmentNumber = await GenerateAdjustmentNumberAsync();
            adjustment.CreatedBy = userId;
            adjustment.CreatedAt = DateTime.UtcNow;

            _context.StockAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();

            // 更新庫存
            foreach (var item in adjustment.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == adjustment.WarehouseId);

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        ProductId = item.ProductId,
                        WarehouseId = adjustment.WarehouseId,
                        Quantity = 0
                    };
                    _context.Inventories.Add(inventory);
                    await _context.SaveChangesAsync();
                }

                item.BeforeQuantity = inventory.Quantity;
                inventory.Quantity = item.AfterQuantity;
                item.AdjustmentQuantity = item.AfterQuantity - item.BeforeQuantity;
                inventory.UpdatedAt = DateTime.UtcNow;

                // 記錄庫存異動
                var movement = new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = adjustment.WarehouseId,
                    Type = MovementType.Adjustment,
                    Quantity = item.AdjustmentQuantity,
                    BeforeQuantity = item.BeforeQuantity,
                    AfterQuantity = item.AfterQuantity,
                    ReferenceType = "StockAdjustment",
                    ReferenceNumber = adjustment.AdjustmentNumber,
                    Notes = adjustment.Reason,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMovements.Add(movement);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("庫存調整單 {AdjustmentNumber} 已建立", adjustment.AdjustmentNumber);
            return adjustment;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<(int Count, int TotalItems)> GetTodayStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var query = _context.StockAdjustments
            .Include(a => a.Items)
            .Where(a => a.CreatedAt >= today && a.CreatedAt < tomorrow);

        var count = await query.CountAsync();
        var totalItems = await query.SelectMany(a => a.Items).CountAsync();

        return (count, totalItems);
    }

    /// <summary>
    /// 產生調整單號
    /// </summary>
    private async Task<string> GenerateAdjustmentNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var prefix = $"ADJ{today:yyyyMMdd}";

        var lastAdjustment = await _context.StockAdjustments
            .Where(a => a.AdjustmentNumber.StartsWith(prefix))
            .OrderByDescending(a => a.AdjustmentNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastAdjustment != null && lastAdjustment.AdjustmentNumber.Length > prefix.Length)
        {
            if (int.TryParse(lastAdjustment.AdjustmentNumber.Substring(prefix.Length), out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
}
