using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 進貨單服務實作
/// </summary>
public class PurchaseReceiptService : IPurchaseReceiptService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PurchaseReceiptService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public PurchaseReceiptService(ApplicationDbContext context, ILogger<PurchaseReceiptService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<PurchaseReceipt>> SearchAsync(string? receiptNumber, int? purchaseOrderId, int? warehouseId, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.PurchaseReceipts
            .Include(r => r.PurchaseOrder).ThenInclude(po => po.Supplier)
            .Include(r => r.Warehouse)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(receiptNumber))
        {
            query = query.Where(r => r.ReceiptNumber.Contains(receiptNumber));
        }

        if (purchaseOrderId.HasValue)
        {
            query = query.Where(r => r.PurchaseOrderId == purchaseOrderId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(r => r.WarehouseId == warehouseId.Value);
        }

        if (startDate.HasValue)
        {
            var start = DateOnly.FromDateTime(startDate.Value);
            query = query.Where(r => r.ReceiptDate >= start);
        }

        if (endDate.HasValue)
        {
            var end = DateOnly.FromDateTime(endDate.Value);
            query = query.Where(r => r.ReceiptDate <= end);
        }

        return await query.OrderByDescending(r => r.CreatedAt).Take(100).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<PurchaseReceipt?> GetByIdAsync(int id)
    {
        return await _context.PurchaseReceipts
            .Include(r => r.PurchaseOrder).ThenInclude(po => po.Supplier)
            .Include(r => r.Warehouse)
            .Include(r => r.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <inheritdoc />
    public async Task<PurchaseReceipt?> GetByReceiptNumberAsync(string receiptNumber)
    {
        return await _context.PurchaseReceipts
            .Include(r => r.PurchaseOrder).ThenInclude(po => po.Supplier)
            .Include(r => r.Warehouse)
            .Include(r => r.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber);
    }

    /// <inheritdoc />
    public async Task<PurchaseReceipt> CreateAsync(PurchaseReceipt receipt, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 產生進貨單號
            receipt.ReceiptNumber = await GenerateReceiptNumberAsync();
            receipt.CreatedBy = userId;
            receipt.CreatedAt = DateTime.UtcNow;

            _context.PurchaseReceipts.Add(receipt);
            await _context.SaveChangesAsync();

            // 更新庫存
            foreach (var item in receipt.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == receipt.WarehouseId);

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        ProductId = item.ProductId,
                        WarehouseId = receipt.WarehouseId,
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
                    WarehouseId = receipt.WarehouseId,
                    Type = MovementType.In,
                    Quantity = item.Quantity,
                    BeforeQuantity = beforeQuantity,
                    AfterQuantity = inventory.Quantity,
                    ReferenceType = "PurchaseReceipt",
                    ReferenceNumber = receipt.ReceiptNumber,
                    Notes = "採購進貨",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMovements.Add(movement);
            }

            // 更新採購單明細的已收數量
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Items)
                .FirstOrDefaultAsync(po => po.Id == receipt.PurchaseOrderId);

            if (purchaseOrder != null)
            {
                foreach (var receiptItem in receipt.Items)
                {
                    var poItem = purchaseOrder.Items.FirstOrDefault(i => i.Id == receiptItem.PurchaseOrderItemId);
                    if (poItem != null)
                    {
                        poItem.ReceivedQuantity += receiptItem.Quantity;
                    }
                }

                // 檢查是否全部收貨完成
                var allReceived = purchaseOrder.Items.All(i => i.ReceivedQuantity >= i.Quantity);
                if (allReceived)
                {
                    purchaseOrder.Status = PurchaseOrderStatus.Completed;
                }
                else if (purchaseOrder.Items.Any(i => i.ReceivedQuantity > 0))
                {
                    purchaseOrder.Status = PurchaseOrderStatus.PartialReceived;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("進貨單 {ReceiptNumber} 已建立並更新庫存", receipt.ReceiptNumber);
            return receipt;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<List<PurchaseReceipt>> GetByPurchaseOrderAsync(int purchaseOrderId)
    {
        return await _context.PurchaseReceipts
            .Include(r => r.Warehouse)
            .Include(r => r.Items).ThenInclude(i => i.Product)
            .Where(r => r.PurchaseOrderId == purchaseOrderId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<(int Count, int TotalItems)> GetTodayStatsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = _context.PurchaseReceipts
            .Include(r => r.Items)
            .Where(r => r.ReceiptDate == today);

        var count = await query.CountAsync();
        var totalItems = await query.SelectMany(r => r.Items).SumAsync(i => i.Quantity);

        return (count, totalItems);
    }

    /// <summary>
    /// 產生進貨單號
    /// </summary>
    private async Task<string> GenerateReceiptNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var prefix = $"RCV{today:yyyyMMdd}";

        var lastReceipt = await _context.PurchaseReceipts
            .Where(r => r.ReceiptNumber.StartsWith(prefix))
            .OrderByDescending(r => r.ReceiptNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastReceipt != null && lastReceipt.ReceiptNumber.Length > prefix.Length)
        {
            if (int.TryParse(lastReceipt.ReceiptNumber.Substring(prefix.Length), out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
}
