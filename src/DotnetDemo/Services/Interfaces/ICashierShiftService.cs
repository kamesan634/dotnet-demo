using DotnetDemo.Models.Entities;

namespace DotnetDemo.Services.Interfaces;

/// <summary>
/// 收銀班別服務介面
/// </summary>
public interface ICashierShiftService
{
    /// <summary>
    /// 搜尋班別
    /// </summary>
    Task<List<CashierShift>> SearchAsync(int? storeId, int? cashierId, CashierShiftStatus? status, DateTime? from, DateTime? to);

    /// <summary>
    /// 依編號取得班別
    /// </summary>
    Task<CashierShift?> GetByIdAsync(int id);

    /// <summary>
    /// 取得收銀員目前進行中的班別
    /// </summary>
    Task<CashierShift?> GetCurrentShiftAsync(int cashierId);

    /// <summary>
    /// 開班
    /// </summary>
    Task<(CashierShift? Shift, string? Error)> OpenShiftAsync(int storeId, int cashierId, decimal openingAmount);

    /// <summary>
    /// 結班
    /// </summary>
    Task<(bool Success, string? Error)> CloseShiftAsync(int shiftId, decimal closingAmount, string? notes);

    /// <summary>
    /// 更新班別銷售統計
    /// </summary>
    Task<bool> UpdateSalesStatsAsync(int shiftId, decimal cashAmount, decimal nonCashAmount);

    /// <summary>
    /// 取得班別銷售摘要
    /// </summary>
    Task<ShiftSummary> GetShiftSummaryAsync(int shiftId);
}

/// <summary>
/// 班別銷售摘要
/// </summary>
public class ShiftSummary
{
    /// <summary>交易筆數</summary>
    public int TransactionCount { get; set; }
    /// <summary>現金銷售</summary>
    public decimal CashSales { get; set; }
    /// <summary>非現金銷售</summary>
    public decimal NonCashSales { get; set; }
    /// <summary>銷售總額</summary>
    public decimal TotalSales { get; set; }
    /// <summary>退貨金額</summary>
    public decimal RefundAmount { get; set; }
    /// <summary>淨銷售額</summary>
    public decimal NetSales { get; set; }
}
