namespace DotnetDemo.Models.Entities;

/// <summary>
/// 報表排程頻率
/// </summary>
public enum ReportFrequency
{
    /// <summary>每日</summary>
    Daily = 1,
    /// <summary>每週</summary>
    Weekly = 2,
    /// <summary>每月</summary>
    Monthly = 3
}

/// <summary>
/// 報表排程
/// </summary>
public class ReportSchedule
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 排程名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型 (Sales, Inventory, Purchasing, Profit 等)
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 排程頻率
    /// </summary>
    public ReportFrequency Frequency { get; set; }

    /// <summary>
    /// 執行時間 (HH:mm 格式)
    /// </summary>
    public string ExecutionTime { get; set; } = "08:00";

    /// <summary>
    /// 週幾執行 (1-7，僅 Weekly 使用)
    /// </summary>
    public int? DayOfWeek { get; set; }

    /// <summary>
    /// 每月幾號執行 (1-31，僅 Monthly 使用)
    /// </summary>
    public int? DayOfMonth { get; set; }

    /// <summary>
    /// 報表參數 (JSON 格式)
    /// </summary>
    public string? Parameters { get; set; }

    /// <summary>
    /// 收件人 Email (多個用逗號分隔)
    /// </summary>
    public string Recipients { get; set; } = string.Empty;

    /// <summary>
    /// 匯出格式 (Excel, PDF, CSV)
    /// </summary>
    public string ExportFormat { get; set; } = "Excel";

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 上次執行時間
    /// </summary>
    public DateTime? LastExecutedAt { get; set; }

    /// <summary>
    /// 上次執行結果
    /// </summary>
    public string? LastExecutionResult { get; set; }

    /// <summary>
    /// 下次執行時間
    /// </summary>
    public DateTime? NextExecutionAt { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
