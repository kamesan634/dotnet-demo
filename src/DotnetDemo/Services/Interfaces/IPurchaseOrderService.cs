using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 採購單服務介面
/// </summary>
public interface IPurchaseOrderService
{
    /// <summary>
    /// 依條件搜尋採購單
    /// </summary>
    Task<List<PurchaseOrder>> SearchAsync(string? orderNumber, int? supplierId, PurchaseOrderStatus? status);

    /// <summary>
    /// 依編號取得採購單
    /// </summary>
    Task<PurchaseOrder?> GetByIdAsync(int id);

    /// <summary>
    /// 新增採購單
    /// </summary>
    Task<PurchaseOrder> CreateAsync(PurchaseOrder order, int userId);

    /// <summary>
    /// 更新採購單
    /// </summary>
    Task<bool> UpdateAsync(PurchaseOrder order);

    /// <summary>
    /// 刪除採購單
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 核准採購單
    /// </summary>
    Task<bool> ApproveAsync(int id, int userId);

    /// <summary>
    /// 取消採購單
    /// </summary>
    Task<bool> CancelAsync(int id);

    /// <summary>
    /// 產生採購單號
    /// </summary>
    Task<string> GenerateOrderNumberAsync();
}
