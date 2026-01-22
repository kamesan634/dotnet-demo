using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 訂單服務實作
/// </summary>
public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderService> _logger;

    /// <summary>
    /// 建構子
    /// </summary>
    public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Order>> SearchAsync(string? orderNumber, OrderStatus? status, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            query = query.Where(o => o.OrderNumber.Contains(orderNumber));
        }

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            var end = endDate.Value.AddDays(1);
            query = query.Where(o => o.CreatedAt < end);
        }

        return await query.OrderByDescending(o => o.CreatedAt).Take(100).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Store)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payments).ThenInclude(p => p.PaymentMethod)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    /// <inheritdoc />
    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payments).ThenInclude(p => p.PaymentMethod)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    /// <inheritdoc />
    public async Task<Order> CreateAsync(Order order)
    {
        order.OrderNumber = await GenerateOrderNumberAsync();
        order.CreatedAt = DateTime.UtcNow;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        _logger.LogInformation("訂單 {OrderNumber} 已建立", order.OrderNumber);
        return order;
    }

    /// <inheritdoc />
    public async Task<Order> CreateWithInventoryDeductionAsync(Order order, int warehouseId, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 設定訂單資料
            order.OrderNumber = await GenerateOrderNumberAsync();
            order.CreatedAt = DateTime.UtcNow;
            order.CreatedBy = userId;

            // 計算訂單金額
            foreach (var item in order.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    item.ProductName = product.Name;
                    item.CostPrice = product.CostPrice;
                    item.SubTotal = item.UnitPrice * item.Quantity - item.DiscountAmount;
                }
            }
            order.SubTotal = order.Items.Sum(i => i.SubTotal);
            order.TotalAmount = order.SubTotal - order.DiscountAmount + order.TaxAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 扣減庫存
            await DeductInventoryForOrderAsync(order, warehouseId, userId);

            await transaction.CommitAsync();

            _logger.LogInformation("訂單 {OrderNumber} 已建立並扣減庫存", order.OrderNumber);
            return order;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("訂單 {OrderNumber} 狀態更新為 {Status}", order.OrderNumber, status);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateStatusWithInventoryAsync(int id, OrderStatus status, int warehouseId, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return false;

        var previousStatus = order.Status;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            // 處理庫存異動
            if (status == OrderStatus.Completed && previousStatus != OrderStatus.Completed)
            {
                // 訂單完成時扣減庫存
                await DeductInventoryForOrderAsync(order, warehouseId, userId);
            }
            else if ((status == OrderStatus.Cancelled || status == OrderStatus.Refunded)
                     && previousStatus == OrderStatus.Completed)
            {
                // 取消或退貨時回補庫存
                await RestoreInventoryForOrderAsync(order, warehouseId, userId);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("訂單 {OrderNumber} 狀態更新為 {Status} 並處理庫存", order.OrderNumber, status);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<(int Count, decimal Total)> GetTodayStatsAsync()
    {
        var today = DateTime.UtcNow.Date;
        var query = _context.Orders
            .Where(o => o.CreatedAt >= today && o.Status == OrderStatus.Completed);

        var count = await query.CountAsync();
        var total = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        return (count, total);
    }

    /// <inheritdoc />
    public async Task<List<Order>> GetRecentAsync(int count)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> DeductInventoryAsync(int orderId, int warehouseId, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await DeductInventoryForOrderAsync(order, warehouseId, userId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("訂單 {OrderNumber} 庫存已扣減", order.OrderNumber);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> RestoreInventoryAsync(int orderId, int warehouseId, int userId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return false;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await RestoreInventoryForOrderAsync(order, warehouseId, userId);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("訂單 {OrderNumber} 庫存已回補", order.OrderNumber);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var today = DateTime.UtcNow.Date;
        var prefix = $"ORD{today:yyyyMMdd}";

        var lastOrder = await _context.Orders
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

    /// <summary>
    /// 扣減訂單庫存 (內部方法)
    /// </summary>
    private async Task DeductInventoryForOrderAsync(Order order, int warehouseId, int userId)
    {
        foreach (var item in order.Items)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == warehouseId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductId = item.ProductId,
                    WarehouseId = warehouseId,
                    Quantity = 0
                };
                _context.Inventories.Add(inventory);
            }

            var beforeQuantity = inventory.Quantity;
            inventory.Quantity -= item.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            // 記錄庫存異動
            var movement = new InventoryMovement
            {
                ProductId = item.ProductId,
                WarehouseId = warehouseId,
                Type = MovementType.Out,
                Quantity = -item.Quantity,
                BeforeQuantity = beforeQuantity,
                AfterQuantity = inventory.Quantity,
                ReferenceType = "Order",
                ReferenceNumber = order.OrderNumber,
                Notes = $"銷售出庫",
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.InventoryMovements.Add(movement);
        }
    }

    /// <summary>
    /// 回補訂單庫存 (內部方法)
    /// </summary>
    private async Task RestoreInventoryForOrderAsync(Order order, int warehouseId, int userId)
    {
        foreach (var item in order.Items)
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.WarehouseId == warehouseId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    ProductId = item.ProductId,
                    WarehouseId = warehouseId,
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
                WarehouseId = warehouseId,
                Type = MovementType.In,
                Quantity = item.Quantity,
                BeforeQuantity = beforeQuantity,
                AfterQuantity = inventory.Quantity,
                ReferenceType = "Order",
                ReferenceNumber = order.OrderNumber,
                Notes = order.Status == OrderStatus.Refunded ? "退貨入庫" : "取消訂單入庫",
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.InventoryMovements.Add(movement);
        }
    }
}
