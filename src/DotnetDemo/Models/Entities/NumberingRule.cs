namespace DotnetDemo.Models.Entities;

/// <summary>
/// 編號規則
/// </summary>
public class NumberingRule
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 規則名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 單據類型 (Order, PurchaseOrder, StockTransfer, StockAdjustment, Invoice 等)
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// 前綴
    /// </summary>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    /// 日期格式 (例如: yyyyMMdd, yyMM)
    /// </summary>
    public string? DateFormat { get; set; }

    /// <summary>
    /// 流水號長度
    /// </summary>
    public int SequenceLength { get; set; } = 4;

    /// <summary>
    /// 目前流水號
    /// </summary>
    public int CurrentSequence { get; set; }

    /// <summary>
    /// 重置週期 (Daily, Monthly, Yearly, Never)
    /// </summary>
    public string ResetPeriod { get; set; } = "Daily";

    /// <summary>
    /// 上次重置日期
    /// </summary>
    public DateTime? LastResetDate { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
