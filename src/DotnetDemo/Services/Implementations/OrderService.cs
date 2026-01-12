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
}
