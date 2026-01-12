using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotnetDemo.Data;
using DotnetDemo.Models.Entities;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 報表服務實作
/// </summary>
public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(ApplicationDbContext context, ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<DashboardData> GetDashboardDataAsync()
    {
        var today = DateTime.UtcNow.Date;

        // 今日銷售統計
        var todayOrdersQuery = _context.Orders
            .Where(o => o.CreatedAt >= today && o.Status == OrderStatus.Completed);

        var todaySales = await todayOrdersQuery.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        var todayOrders = await todayOrdersQuery.CountAsync();

        // 商品總數
        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);

        // 低庫存
        var lowStockItems = await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.Quantity < i.Product.SafetyStock && i.Product.IsActive)
            .Select(i => new LowStockItem
            {
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                SafetyStock = i.Product.SafetyStock
            })
            .Take(10)
            .ToListAsync();

        // 最近訂單
        var recentOrders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new RecentOrderItem
            {
                OrderNumber = o.OrderNumber,
                TotalAmount = o.TotalAmount,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();

        return new DashboardData
        {
            TodaySales = todaySales,
            TodayOrders = todayOrders,
            TotalProducts = totalProducts,
            LowStockCount = lowStockItems.Count,
            LowStockItems = lowStockItems,
            RecentOrders = recentOrders
        };
    }

    /// <inheritdoc />
    public async Task<List<SalesDailyReport>> GetSalesDailyReportAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate.AddDays(1))
            .Where(o => o.Status == OrderStatus.Completed)
            .ToListAsync();

        var report = orders
            .GroupBy(o => DateOnly.FromDateTime(o.CreatedAt))
            .Select(g => new SalesDailyReport
            {
                Date = g.Key,
                OrderCount = g.Count(),
                TotalAmount = g.Sum(o => o.TotalAmount),
                DiscountAmount = g.Sum(o => o.DiscountAmount),
                NetAmount = g.Sum(o => o.TotalAmount - o.DiscountAmount)
            })
            .OrderBy(r => r.Date)
            .ToList();

        return report;
    }

    /// <inheritdoc />
    public async Task<List<ProductRankingReport>> GetProductRankingAsync(DateTime startDate, DateTime endDate, int top = 10)
    {
        var items = await _context.OrderItems
            .Include(i => i.Order)
            .Include(i => i.Product)
            .Where(i => i.Order.CreatedAt >= startDate && i.Order.CreatedAt < endDate.AddDays(1))
            .Where(i => i.Order.Status == OrderStatus.Completed)
            .ToListAsync();

        var report = items
            .GroupBy(i => new { i.ProductId, i.Product.Code, i.Product.Name })
            .Select(g => new ProductRankingReport
            {
                ProductCode = g.Key.Code,
                ProductName = g.Key.Name,
                Quantity = g.Sum(i => i.Quantity),
                Amount = g.Sum(i => i.SubTotal)
            })
            .OrderByDescending(r => r.Amount)
            .Take(top)
            .ToList();

        for (int i = 0; i < report.Count; i++)
        {
            report[i].Rank = i + 1;
        }

        return report;
    }

    /// <inheritdoc />
    public async Task<List<InventoryReport>> GetInventoryReportAsync(int? warehouseId, bool lowStockOnly)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
            .Include(i => i.Warehouse)
            .Where(i => i.Product.IsActive)
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == warehouseId.Value);
        }

        if (lowStockOnly)
        {
            query = query.Where(i => i.Quantity < i.Product.SafetyStock);
        }

        var inventories = await query.ToListAsync();

        return inventories.Select(i => new InventoryReport
        {
            ProductCode = i.Product.Code,
            ProductName = i.Product.Name,
            WarehouseName = i.Warehouse.Name,
            Quantity = i.Quantity,
            SafetyStock = i.Product.SafetyStock,
            IsLowStock = i.Quantity < i.Product.SafetyStock,
            StockValue = i.Quantity * i.Product.CostPrice
        }).OrderBy(r => r.ProductCode).ToList();
    }

    /// <inheritdoc />
    public async Task<PurchasingSummary> GetPurchasingSummaryAsync(DateTime startDate, DateTime endDate)
    {
        var orders = await _context.PurchaseOrders
            .Include(o => o.Supplier)
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate.AddDays(1))
            .ToListAsync();

        var topSuppliers = orders
            .GroupBy(o => o.Supplier.Name)
            .Select(g => new SupplierPurchaseItem
            {
                SupplierName = g.Key,
                OrderCount = g.Count(),
                TotalAmount = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(s => s.TotalAmount)
            .Take(5)
            .ToList();

        return new PurchasingSummary
        {
            TotalOrders = orders.Count,
            TotalAmount = orders.Sum(o => o.TotalAmount),
            PendingOrders = orders.Count(o => o.Status == PurchaseOrderStatus.Draft || o.Status == PurchaseOrderStatus.PendingApproval),
            CompletedOrders = orders.Count(o => o.Status == PurchaseOrderStatus.Completed),
            TopSuppliers = topSuppliers
        };
    }

    /// <inheritdoc />
    public async Task<List<CustomerSpendingReport>> GetCustomerSpendingReportAsync(DateTime startDate, DateTime endDate, int top = 10)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.CustomerId != null)
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate.AddDays(1))
            .Where(o => o.Status == OrderStatus.Completed)
            .ToListAsync();

        var report = orders
            .GroupBy(o => new { o.CustomerId, o.Customer!.Code, o.Customer.Name })
            .Select(g => new CustomerSpendingReport
            {
                CustomerCode = g.Key.Code,
                CustomerName = g.Key.Name,
                OrderCount = g.Count(),
                TotalSpent = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(r => r.TotalSpent)
            .Take(top)
            .ToList();

        for (int i = 0; i < report.Count; i++)
        {
            report[i].Rank = i + 1;
        }

        return report;
    }
}
