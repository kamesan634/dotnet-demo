namespace DotnetDemo.Models.Entities;

/// <summary>
/// 收銀班別狀態
/// </summary>
public enum CashierShiftStatus
{
    /// <summary>進行中</summary>
    Open = 1,
    /// <summary>已結班</summary>
    Closed = 2
}

/// <summary>
/// 收銀班別
/// </summary>
public class CashierShift
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 門市編號
    /// </summary>
    public int StoreId { get; set; }

    /// <summary>
    /// 門市
    /// </summary>
    public Store Store { get; set; } = null!;

    /// <summary>
    /// 收銀員編號
    /// </summary>
    public int CashierId { get; set; }

    /// <summary>
    /// 收銀員
    /// </summary>
    public ApplicationUser Cashier { get; set; } = null!;

    /// <summary>
    /// 開班時間
    /// </summary>
    public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 結班時間
    /// </summary>
    public DateTime? ClosedAt { get; set; }

    /// <summary>
    /// 開班金額
    /// </summary>
    public decimal OpeningAmount { get; set; }

    /// <summary>
    /// 結班金額
    /// </summary>
    public decimal? ClosingAmount { get; set; }

    /// <summary>
    /// 現金銷售總額
    /// </summary>
    public decimal CashSalesTotal { get; set; }

    /// <summary>
    /// 非現金銷售總額
    /// </summary>
    public decimal NonCashSalesTotal { get; set; }

    /// <summary>
    /// 交易筆數
    /// </summary>
    public int TransactionCount { get; set; }

    /// <summary>
    /// 差額
    /// </summary>
    public decimal? Difference { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public CashierShiftStatus Status { get; set; } = CashierShiftStatus.Open;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }
}
