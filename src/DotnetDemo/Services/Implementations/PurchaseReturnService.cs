using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 採購退貨服務實作
/// </summary>
public class PurchaseReturnService : IPurchaseReturnService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public PurchaseReturnService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<PurchaseReturn>> SearchAsync(int? supplierId, PurchaseReturnStatus? status, DateTime? from, DateTime? to)
    {
        var query = _context.PurchaseReturns
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
            .AsQueryable();

        if (supplierId.HasValue)
            query = query.Where(x => x.SupplierId == supplierId.Value);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (from.HasValue)
            query = query.Where(x => x.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.CreatedAt <= to.Value);

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<PurchaseReturn?> GetByIdAsync(int id)
    {
        return await _context.PurchaseReturns
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .Include(x => x.PurchaseReceipt)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public async Task<PurchaseReturn> CreateAsync(PurchaseReturn purchaseReturn, int userId)
    {
        purchaseReturn.ReturnNumber = await GenerateReturnNumberAsync();
        purchaseReturn.CreatedBy = userId;
        purchaseReturn.Status = PurchaseReturnStatus.Pending;

        purchaseReturn.TotalAmount = purchaseReturn.Items.Sum(x => x.SubTotal);

        _context.PurchaseReturns.Add(purchaseReturn);
        await _context.SaveChangesAsync();

        return purchaseReturn;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> ConfirmAsync(int id, int userId)
    {
        var purchaseReturn = await _context.PurchaseReturns
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (purchaseReturn == null)
            return (false, "退貨單不存在");

        if (purchaseReturn.Status != PurchaseReturnStatus.Pending)
            return (false, "退貨單狀態不正確");

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in purchaseReturn.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseId == purchaseReturn.WarehouseId);

                if (inventory == null)
                    return (false, $"商品庫存不存在");

                if (inventory.Quantity < item.Quantity)
                    return (false, $"庫存不足，無法退貨");

                inventory.Quantity -= item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;

                var movement = new InventoryMovement
                {
                    ProductId = item.ProductId,
                    WarehouseId = purchaseReturn.WarehouseId,
                    Type = MovementType.Out,
                    Quantity = item.Quantity,
                    BeforeQuantity = inventory.Quantity + item.Quantity,
                    AfterQuantity = inventory.Quantity,
                    ReferenceType = "PurchaseReturn",
                    ReferenceNumber = purchaseReturn.ReturnNumber,
                    Notes = $"採購退貨：{purchaseReturn.Reason}",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.InventoryMovements.Add(movement);
            }

            purchaseReturn.Status = PurchaseReturnStatus.Confirmed;
            purchaseReturn.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"確認退貨失敗：{ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CompleteAsync(int id, int userId)
    {
        var purchaseReturn = await _context.PurchaseReturns.FindAsync(id);
        if (purchaseReturn == null || purchaseReturn.Status != PurchaseReturnStatus.Confirmed)
            return false;

        purchaseReturn.Status = PurchaseReturnStatus.Returned;
        purchaseReturn.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> CancelAsync(int id, int userId)
    {
        var purchaseReturn = await _context.PurchaseReturns
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (purchaseReturn == null)
            return (false, "退貨單不存在");

        if (purchaseReturn.Status == PurchaseReturnStatus.Returned)
            return (false, "已完成的退貨單無法取消");

        if (purchaseReturn.Status == PurchaseReturnStatus.Confirmed)
        {
            foreach (var item in purchaseReturn.Items)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(x => x.ProductId == item.ProductId && x.WarehouseId == purchaseReturn.WarehouseId);

                if (inventory != null)
                {
                    inventory.Quantity += item.Quantity;
                    inventory.UpdatedAt = DateTime.UtcNow;

                    var movement = new InventoryMovement
                    {
                        ProductId = item.ProductId,
                        WarehouseId = purchaseReturn.WarehouseId,
                        Type = MovementType.In,
                        Quantity = item.Quantity,
                        BeforeQuantity = inventory.Quantity - item.Quantity,
                        AfterQuantity = inventory.Quantity,
                        ReferenceType = "PurchaseReturn",
                        ReferenceNumber = purchaseReturn.ReturnNumber,
                        Notes = "採購退貨取消",
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.InventoryMovements.Add(movement);
                }
            }
        }

        purchaseReturn.Status = PurchaseReturnStatus.Cancelled;
        purchaseReturn.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (true, null);
    }

    /// <inheritdoc/>
    public async Task<string> GenerateReturnNumberAsync()
    {
        var today = DateTime.Today;
        var count = await _context.PurchaseReturns
            .CountAsync(x => x.CreatedAt >= today);

        return $"PR{DateTime.Now:yyyyMMdd}{(count + 1).ToString().PadLeft(4, '0')}";
    }
}
