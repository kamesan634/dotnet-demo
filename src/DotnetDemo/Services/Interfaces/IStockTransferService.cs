using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 庫存調撥服務介面
/// </summary>
public interface IStockTransferService
{
    /// <summary>
    /// 依條件搜尋調撥單
    /// </summary>
    Task<List<StockTransfer>> SearchAsync(string? transferNumber, int? fromWarehouseId, int? toWarehouseId, StockTransferStatus? status, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// 依編號取得調撥單
    /// </summary>
    Task<StockTransfer?> GetByIdAsync(int id);

    /// <summary>
    /// 建立調撥單
    /// </summary>
    Task<StockTransfer> CreateAsync(StockTransfer transfer, int userId);

    /// <summary>
    /// 出庫確認
    /// </summary>
    Task<bool> ShipAsync(int id, int userId);

    /// <summary>
    /// 入庫確認
    /// </summary>
    Task<bool> ReceiveAsync(int id, int userId);

    /// <summary>
    /// 取消調撥單
    /// </summary>
    Task<bool> CancelAsync(int id, int userId);

    /// <summary>
    /// 取得今日調撥統計
    /// </summary>
    Task<(int Count, int TotalItems)> GetTodayStatsAsync();
}
