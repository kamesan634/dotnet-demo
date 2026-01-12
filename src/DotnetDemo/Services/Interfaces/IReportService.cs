namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 報表服務介面
/// </summary>
public interface IReportService
{
    /// <summary>
    /// 取得儀表板資料
    /// </summary>
    Task<DashboardData> GetDashboardDataAsync();

    /// <summary>
    /// 取得銷售日報
    /// </summary>
    Task<List<SalesDailyReport>> GetSalesDailyReportAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// 取得商品銷售排行
    /// </summary>
    Task<List<ProductRankingReport>> GetProductRankingAsync(DateTime startDate, DateTime endDate, int top = 10);

    /// <summary>
    /// 取得庫存報表
    /// </summary>
    Task<List<InventoryReport>> GetInventoryReportAsync(int? warehouseId, bool lowStockOnly);

    /// <summary>
    /// 取得採購統計
    /// </summary>
    Task<PurchasingSummary> GetPurchasingSummaryAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// 取得客戶消費統計
    /// </summary>
    Task<List<CustomerSpendingReport>> GetCustomerSpendingReportAsync(DateTime startDate, DateTime endDate, int top = 10);
}

/// <summary>
/// 儀表板資料
/// </summary>
public class DashboardData
{
    public decimal TodaySales { get; set; }
    public int TodayOrders { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockCount { get; set; }
    public List<LowStockItem> LowStockItems { get; set; } = new();
    public List<RecentOrderItem> RecentOrders { get; set; } = new();
}

public class LowStockItem
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int SafetyStock { get; set; }
}

public class RecentOrderItem
{
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 銷售日報
/// </summary>
public class SalesDailyReport
{
    public DateOnly Date { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
}

/// <summary>
/// 商品銷售排行
/// </summary>
public class ProductRankingReport
{
    public int Rank { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
}

/// <summary>
/// 庫存報表
/// </summary>
public class InventoryReport
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int SafetyStock { get; set; }
    public bool IsLowStock { get; set; }
    public decimal StockValue { get; set; }
}

/// <summary>
/// 採購統計
/// </summary>
public class PurchasingSummary
{
    public int TotalOrders { get; set; }
    public decimal TotalAmount { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public List<SupplierPurchaseItem> TopSuppliers { get; set; } = new();
}

public class SupplierPurchaseItem
{
    public string SupplierName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// 客戶消費報表
/// </summary>
public class CustomerSpendingReport
{
    public int Rank { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}
