using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 進貨單服務介面
/// </summary>
public interface IPurchaseReceiptService
{
    /// <summary>
    /// 依條件搜尋進貨單
    /// </summary>
    /// <param name="receiptNumber">進貨單號</param>
    /// <param name="purchaseOrderId">採購單編號</param>
    /// <param name="warehouseId">倉庫編號</param>
    /// <param name="startDate">開始日期</param>
    /// <param name="endDate">結束日期</param>
    Task<List<PurchaseReceipt>> SearchAsync(string? receiptNumber, int? purchaseOrderId, int? warehouseId, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// 依編號取得進貨單
    /// </summary>
    /// <param name="id">進貨單編號</param>
    Task<PurchaseReceipt?> GetByIdAsync(int id);

    /// <summary>
    /// 依進貨單號取得進貨單
    /// </summary>
    /// <param name="receiptNumber">進貨單號</param>
    Task<PurchaseReceipt?> GetByReceiptNumberAsync(string receiptNumber);

    /// <summary>
    /// 建立進貨單並更新庫存
    /// </summary>
    /// <param name="receipt">進貨單</param>
    /// <param name="userId">操作人員編號</param>
    Task<PurchaseReceipt> CreateAsync(PurchaseReceipt receipt, int userId);

    /// <summary>
    /// 依採購單取得進貨記錄
    /// </summary>
    /// <param name="purchaseOrderId">採購單編號</param>
    Task<List<PurchaseReceipt>> GetByPurchaseOrderAsync(int purchaseOrderId);

    /// <summary>
    /// 取得今日進貨統計
    /// </summary>
    Task<(int Count, int TotalItems)> GetTodayStatsAsync();
}
