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
    /// 新增訂單並扣減庫存
    /// </summary>
    /// <param name="order">訂單</param>
    /// <param name="warehouseId">出貨倉庫編號</param>
    /// <param name="userId">操作人員編號</param>
    Task<Order> CreateWithInventoryDeductionAsync(Order order, int warehouseId, int userId);

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    Task<bool> UpdateStatusAsync(int id, OrderStatus status);

    /// <summary>
    /// 更新訂單狀態並處理庫存
    /// </summary>
    /// <param name="id">訂單編號</param>
    /// <param name="status">新狀態</param>
    /// <param name="warehouseId">倉庫編號</param>
    /// <param name="userId">操作人員編號</param>
    Task<bool> UpdateStatusWithInventoryAsync(int id, OrderStatus status, int warehouseId, int userId);

    /// <summary>
    /// 取得今日訂單統計
    /// </summary>
    Task<(int Count, decimal Total)> GetTodayStatsAsync();

    /// <summary>
    /// 取得最近訂單
    /// </summary>
    Task<List<Order>> GetRecentAsync(int count);

    /// <summary>
    /// 扣減庫存
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="warehouseId">出貨倉庫編號</param>
    /// <param name="userId">操作人員編號</param>
    Task<bool> DeductInventoryAsync(int orderId, int warehouseId, int userId);

    /// <summary>
    /// 回補庫存 (取消/退貨時使用)
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <param name="warehouseId">入庫倉庫編號</param>
    /// <param name="userId">操作人員編號</param>
    Task<bool> RestoreInventoryAsync(int orderId, int warehouseId, int userId);
}
