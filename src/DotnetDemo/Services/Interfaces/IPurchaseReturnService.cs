using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 採購退貨服務介面
/// </summary>
public interface IPurchaseReturnService
{
    /// <summary>
    /// 搜尋退貨單
    /// </summary>
    Task<List<PurchaseReturn>> SearchAsync(int? supplierId, PurchaseReturnStatus? status, DateTime? from, DateTime? to);

    /// <summary>
    /// 依編號取得退貨單
    /// </summary>
    Task<PurchaseReturn?> GetByIdAsync(int id);

    /// <summary>
    /// 建立退貨單
    /// </summary>
    Task<PurchaseReturn> CreateAsync(PurchaseReturn purchaseReturn, int userId);

    /// <summary>
    /// 確認退貨 (扣減庫存)
    /// </summary>
    Task<(bool Success, string? Error)> ConfirmAsync(int id, int userId);

    /// <summary>
    /// 完成退貨
    /// </summary>
    Task<bool> CompleteAsync(int id, int userId);

    /// <summary>
    /// 取消退貨
    /// </summary>
    Task<(bool Success, string? Error)> CancelAsync(int id, int userId);

    /// <summary>
    /// 產生退貨單號
    /// </summary>
    Task<string> GenerateReturnNumberAsync();
}
