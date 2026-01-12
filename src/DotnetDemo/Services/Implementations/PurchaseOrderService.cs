using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 採購單服務實作
/// </summary>
public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PurchaseOrderService> _logger;

    public PurchaseOrderService(ApplicationDbContext context, ILogger<PurchaseOrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<PurchaseOrder>> SearchAsync(string? orderNumber, int? supplierId, PurchaseOrderStatus? status)
    {
        var query = _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Include(o => o.Warehouse)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            query = query.Where(o => o.OrderNumber.Contains(orderNumber));
        }

        if (supplierId.HasValue)
        {
            query = query.Where(o => o.SupplierId == supplierId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return await query.OrderByDescending(o => o.CreatedAt).Take(100).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<PurchaseOrder?> GetByIdAsync(int id)
    {
        return await _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Include(o => o.Warehouse)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <inheritdoc />
    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder order, int userId)
    {
        order.OrderNumber = await GenerateOrderNumberAsync();
        order.Status = PurchaseOrderStatus.Draft;
        order.CreatedBy = userId;
        order.CreatedAt = DateTime.UtcNow;

        // 計算總金額
        order.TotalAmount = order.Items.Sum(i => i.SubTotal);

        _context.PurchaseOrders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("採購單 {OrderNumber} 已建立", order.OrderNumber);
        return order;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(PurchaseOrder order)
    {
        var existing = await _context.PurchaseOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        if (existing == null) return false;
        if (existing.Status != PurchaseOrderStatus.Draft) return false;

        existing.SupplierId = order.SupplierId;
        existing.WarehouseId = order.WarehouseId;
        existing.ExpectedDeliveryDate = order.ExpectedDeliveryDate;
        existing.Notes = order.Notes;
        existing.UpdatedAt = DateTime.UtcNow;

        // 更新明細
        _context.PurchaseOrderItems.RemoveRange(existing.Items);
        foreach (var item in order.Items)
        {
            item.PurchaseOrderId = existing.Id;
            _context.PurchaseOrderItems.Add(item);
        }

        existing.TotalAmount = order.Items.Sum(i => i.SubTotal);

        await _context.SaveChangesAsync();

        _logger.LogInformation("採購單 {OrderNumber} 已更新", existing.OrderNumber);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return false;
        if (order.Status != PurchaseOrderStatus.Draft) return false;

        _context.PurchaseOrderItems.RemoveRange(order.Items);
        _context.PurchaseOrders.Remove(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("採購單 {OrderNumber} 已刪除", order.OrderNumber);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ApproveAsync(int id, int userId)
    {
        var order = await _context.PurchaseOrders.FindAsync(id);
        if (order == null) return false;
        if (order.Status != PurchaseOrderStatus.Draft && order.Status != PurchaseOrderStatus.PendingApproval) return false;

        order.Status = PurchaseOrderStatus.Approved;
        order.ApprovedBy = userId;
        order.ApprovedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("採購單 {OrderNumber} 已核准", order.OrderNumber);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CancelAsync(int id)
    {
        var order = await _context.PurchaseOrders.FindAsync(id);
        if (order == null) return false;
        if (order.Status == PurchaseOrderStatus.Completed || order.Status == PurchaseOrderStatus.Cancelled) return false;

        order.Status = PurchaseOrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("採購單 {OrderNumber} 已取消", order.OrderNumber);
        return true;
    }

    /// <inheritdoc />
    public async Task<string> GenerateOrderNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var prefix = $"PO{today:yyyyMMdd}";

        var lastOrder = await _context.PurchaseOrders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastOrder != null && lastOrder.OrderNumber.Length > prefix.Length)
        {
            if (int.TryParse(lastOrder.OrderNumber.Substring(prefix.Length), out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }
}
