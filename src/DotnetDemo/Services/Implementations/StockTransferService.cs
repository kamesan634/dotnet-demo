using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 庫存調撥服務實作
/// </summary>
public class StockTransferService : IStockTransferService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StockTransferService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public StockTransferService(ApplicationDbContext context, ILogger<StockTransferService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<StockTransfer>> SearchAsync(string? transferNumber, int? fromWarehouseId, int? toWarehouseId, StockTransferStatus? status, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StockTransfers
            .Include(t => t.FromWarehouse)
            .Include(t => t.ToWarehouse)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(transferNumber))
        {
            query = query.Where(t => t.TransferNumber.Contains(transferNumber));
        }

        if (fromWarehouseId.HasValue)
        {
            query = query.Where(t => t.FromWarehouseId == fromWarehouseId.Value);
        }

        if (toWarehouseId.HasValue)
        {
            query = query.Where(t => t.ToWarehouseId == toWarehouseId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value.AddDays(1);
            query = query.Where(t => t.CreatedAt < end);
        }

        return await query.OrderByDescending(t => t.CreatedAt).Take(100).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<StockTransfer?> GetByIdAsync(int id)
    {
        return await _context.StockTransfers
            .Include(t => t.FromWarehouse)
            .Include(t => t.ToWarehouse)
            .Include(t => t.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    /// <inheritdoc />
    public async Task<StockTransfer> CreateAsync(StockTransfer transfer, int userId)
    {
        transfer.TransferNumber = await GenerateTransferNumberAsync();
        transfer.CreatedBy = userId;
        transfer.CreatedAt = DateTime.UtcNow;
        transfer.Status = StockTransferStatus.Pending;

        _context.StockTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("調撥單 {TransferNumber} 已建立", transfer.TransferNumber);
        return transfer;
    }

    /// <inheritdoc />
    public async Task<bool> ShipAsync(int id, int userId)
    {
        var transfer = await _context.StockTransfers
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transfer == null || transfer.Status != StockTransferStatus.Pending)
            return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            transfer.Status = StockTransferStatus.Shipped;
            transfer.ShippedBy = userId;
            transfer.ShippedAt = DateTime.UtcNow;

            // 從來源倉庫扣除庫存
            foreach (var item in transfer.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == transfer.FromWarehouseId);

                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var beforeQuantity = inventory.Quantity;
                inventory.Quantity -= item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;

                // 記錄庫存異動
                var movement = new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = transfer.FromWarehouseId,
                    Type = MovementType.TransferOut,
                    Quantity = -item.Quantity,
                    BeforeQuantity = beforeQuantity,
                    AfterQuantity = inventory.Quantity,
                    ReferenceType = "StockTransfer",
                    ReferenceNumber = transfer.TransferNumber,
                    Notes = "調撥出庫",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMovements.Add(movement);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("調撥單 {TransferNumber} 已出庫", transfer.TransferNumber);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ReceiveAsync(int id, int userId)
    {
        var transfer = await _context.StockTransfers
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transfer == null || transfer.Status != StockTransferStatus.Shipped)
            return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            transfer.Status = StockTransferStatus.Received;
            transfer.ReceivedBy = userId;
            transfer.ReceivedAt = DateTime.UtcNow;

            // 加入目標倉庫庫存
            foreach (var item in transfer.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == transfer.ToWarehouseId);

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        ProductId = item.ProductId,
                        WarehouseId = transfer.ToWarehouseId,
                        Quantity = 0
                    };
                    _context.Inventories.Add(inventory);
                }

                var beforeQuantity = inventory.Quantity;
                inventory.Quantity += item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;

                // 記錄庫存異動
                var movement = new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = transfer.ToWarehouseId,
                    Type = MovementType.TransferIn,
                    Quantity = item.Quantity,
                    BeforeQuantity = beforeQuantity,
                    AfterQuantity = inventory.Quantity,
                    ReferenceType = "StockTransfer",
                    ReferenceNumber = transfer.TransferNumber,
                    Notes = "調撥入庫",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMovements.Add(movement);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("調撥單 {TransferNumber} 已入庫", transfer.TransferNumber);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(int id, int userId)
    {
        var transfer = await _context.StockTransfers.FindAsync(id);

        if (transfer == null || transfer.Status != StockTransferStatus.Pending)
            return false;

        transfer.Status = StockTransferStatus.Cancelled;
        await _context.SaveChangesAsync();

        _logger.LogInformation("調撥單 {TransferNumber} 已取消", transfer.TransferNumber);
        return true;
    }

    /// <inheritdoc />
    public async Task<(int Count, int TotalItems)> GetTodayStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var query = _context.StockTransfers
            .Include(t => t.Items)
            .Where(t => t.CreatedAt >= today && t.CreatedAt < tomorrow);

        var count = await query.CountAsync();
        var totalItems = await query.SelectMany(t => t.Items).SumAsync(i => i.Quantity);

        return (count, totalItems);
    }

    /// <summary>
    /// 產生調撥單號
    /// </summary>
    private async Task<string> GenerateTransferNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var prefix = $"TRF{today:yyyyMMdd}";

        var lastTransfer = await _context.StockTransfers
            .Where(t => t.TransferNumber.StartsWith(prefix))
            .OrderByDescending(t => t.TransferNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastTransfer != null && lastTransfer.TransferNumber.Length > prefix.Length)
        {
            if (int.TryParse(lastTransfer.TransferNumber.Substring(prefix.Length), out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
}
