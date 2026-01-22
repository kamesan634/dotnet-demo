using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 庫存調整服務介面
/// </summary>
public interface IStockAdjustmentService
{
    /// <summary>
    /// 依條件搜尋庫存調整單
    /// </summary>
    Task<List<StockAdjustment>> SearchAsync(string? adjustmentNumber, int? warehouseId, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// 依編號取得庫存調整單
    /// </summary>
    Task<StockAdjustment?> GetByIdAsync(int id);

    /// <summary>
    /// 建立庫存調整單並更新庫存
    /// </summary>
    Task<StockAdjustment> CreateAsync(StockAdjustment adjustment, int userId);

    /// <summary>
    /// 取得今日調整統計
    /// </summary>
    Task<(int Count, int TotalItems)> GetTodayStatsAsync();
}
