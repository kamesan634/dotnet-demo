using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 庫存盤點服務介面
/// </summary>
public interface IStockCountService
{
    /// <summary>
    /// 搜尋盤點單
    /// </summary>
    Task<List<StockCount>> SearchAsync(int? warehouseId, StockCountStatus? status, DateTime? from, DateTime? to);

    /// <summary>
    /// 依編號取得盤點單
    /// </summary>
    Task<StockCount?> GetByIdAsync(int id);

    /// <summary>
    /// 建立盤點單
    /// </summary>
    Task<StockCount> CreateAsync(StockCount stockCount, int userId);

    /// <summary>
    /// 初始化盤點明細 (依倉庫現有庫存)
    /// </summary>
    Task<bool> InitializeItemsAsync(int stockCountId);

    /// <summary>
    /// 更新盤點數量
    /// </summary>
    Task<bool> UpdateItemQuantityAsync(int itemId, int actualQuantity, int userId, string? reason = null);

    /// <summary>
    /// 完成盤點 (調整庫存)
    /// </summary>
    Task<(bool Success, string? Error)> CompleteAsync(int id, int userId);

    /// <summary>
    /// 取消盤點
    /// </summary>
    Task<bool> CancelAsync(int id, int userId);

    /// <summary>
    /// 產生盤點單號
    /// </summary>
    Task<string> GenerateCountNumberAsync();
}
