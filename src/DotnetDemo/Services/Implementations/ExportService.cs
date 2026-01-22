using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DotnetDemo.Data;
using DotnetDemo.Services.Interfaces;

namespace DotnetDemo.Services.Implementations;

/// <summary>
/// 匯出服務實作
/// </summary>
public class ExportService : IExportService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// 建構子
    /// </summary>
    public ExportService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, Dictionary<string, string>? columnMappings = null)
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var headers = properties.Select(p =>
            columnMappings?.GetValueOrDefault(p.Name, p.Name) ?? p.Name);
        sb.AppendLine(string.Join("\t", headers));

        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                return value?.ToString()?.Replace("\t", " ").Replace("\n", " ") ?? "";
            });
            sb.AppendLine(string.Join("\t", values));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return Task.FromResult(bytes);
    }

    /// <inheritdoc/>
    public Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data, Dictionary<string, string>? columnMappings = null)
    {
        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var headers = properties.Select(p =>
            EscapeCsvField(columnMappings?.GetValueOrDefault(p.Name, p.Name) ?? p.Name));
        sb.AppendLine(string.Join(",", headers));

        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item);
                return EscapeCsvField(value?.ToString() ?? "");
            });
            sb.AppendLine(string.Join(",", values));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return Task.FromResult(bytes);
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportSalesReportAsync(DateTime from, DateTime to)
    {
        var orders = await _context.Orders
            .Include(o => o.Store)
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to)
            .Where(o => o.Status != Models.Entities.OrderStatus.Cancelled)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var reportData = orders.Select(o => new
        {
            訂單編號 = o.OrderNumber,
            門市 = o.Store?.Name ?? "",
            客戶 = o.Customer?.Name ?? "散客",
            訂單日期 = o.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            商品數 = o.Items.Sum(i => i.Quantity),
            小計 = o.SubTotal,
            折扣 = o.DiscountAmount,
            稅額 = o.TaxAmount,
            總金額 = o.TotalAmount,
            狀態 = GetOrderStatusText(o.Status)
        }).ToList();

        return await ExportToExcelAsync(reportData, "銷售報表");
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportInventoryReportAsync(int? warehouseId = null)
    {
        var query = _context.Inventories
            .Include(i => i.Product)
                .ThenInclude(p => p.Category)
            .Include(i => i.Product)
                .ThenInclude(p => p.Unit)
            .Include(i => i.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(i => i.WarehouseId == warehouseId.Value);
        }

        var inventories = await query.OrderBy(i => i.Product.Code).ToListAsync();

        var reportData = inventories.Select(i => new
        {
            商品代碼 = i.Product.Code,
            商品名稱 = i.Product.Name,
            分類 = i.Product.Category?.Name ?? "",
            單位 = i.Product.Unit?.Name ?? "",
            倉庫 = i.Warehouse?.Name ?? "",
            庫存數量 = i.Quantity,
            安全庫存 = i.Product.SafetyStock,
            庫存狀態 = i.Quantity < i.Product.SafetyStock ? "低於安全庫存" : "正常",
            成本價 = i.Product.CostPrice,
            庫存價值 = i.Product.CostPrice * i.Quantity
        }).ToList();

        return await ExportToExcelAsync(reportData, "庫存報表");
    }

    /// <inheritdoc/>
    public async Task<byte[]> ExportPurchasingReportAsync(DateTime from, DateTime to)
    {
        var purchaseOrders = await _context.PurchaseOrders
            .Include(p => p.Supplier)
            .Include(p => p.Warehouse)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var reportData = purchaseOrders.Select(p => new
        {
            採購單號 = p.OrderNumber,
            供應商 = p.Supplier?.Name ?? "",
            倉庫 = p.Warehouse?.Name ?? "",
            採購日期 = p.CreatedAt.ToString("yyyy-MM-dd"),
            預計到貨 = p.ExpectedDeliveryDate.ToString("yyyy-MM-dd"),
            商品數 = p.Items.Sum(i => i.Quantity),
            總金額 = p.TotalAmount,
            狀態 = GetPurchaseOrderStatusText(p.Status)
        }).ToList();

        return await ExportToExcelAsync(reportData, "採購報表");
    }

    private string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }

    private string GetOrderStatusText(Models.Entities.OrderStatus status) => status switch
    {
        Models.Entities.OrderStatus.Pending => "待處理",
        Models.Entities.OrderStatus.Confirmed => "已確認",
        Models.Entities.OrderStatus.Completed => "已完成",
        Models.Entities.OrderStatus.Cancelled => "已取消",
        Models.Entities.OrderStatus.Refunded => "已退貨",
        _ => "未知"
    };

    private string GetPurchaseOrderStatusText(Models.Entities.PurchaseOrderStatus status) => status switch
    {
        Models.Entities.PurchaseOrderStatus.Draft => "草稿",
        Models.Entities.PurchaseOrderStatus.PendingApproval => "待審核",
        Models.Entities.PurchaseOrderStatus.Approved => "已核准",
        Models.Entities.PurchaseOrderStatus.PartialReceived => "部分收貨",
        Models.Entities.PurchaseOrderStatus.Completed => "已完成",
        Models.Entities.PurchaseOrderStatus.Cancelled => "已取消",
        _ => "未知"
    };
}
