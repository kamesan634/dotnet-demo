using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 訂單服務介面
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// 依條件搜尋訂單
    /// </summary>
    Task<List<Order>> SearchAsync(string? orderNumber, OrderStatus? status, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// 依編號取得訂單
    /// </summary>
    Task<Order?> GetByIdAsync(int id);

    /// <summary>
    /// 依訂單編號取得訂單
    /// </summary>
    Task<Order?> GetByOrderNumberAsync(string orderNumber);

    /// <summary>
    /// 新增訂單
    /// </summary>
    Task<Order> CreateAsync(Order order);

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    Task<bool> UpdateStatusAsync(int id, OrderStatus status);

    /// <summary>
    /// 取得今日訂單統計
    /// </summary>
    Task<(int Count, decimal Total)> GetTodayStatsAsync();

    /// <summary>
    /// 取得最近訂單
    /// </summary>
    Task<List<Order>> GetRecentAsync(int count);
}
