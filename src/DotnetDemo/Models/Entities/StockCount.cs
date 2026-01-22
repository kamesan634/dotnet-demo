namespace DotnetDemo.Models.Entities;

/// <summary>
/// 盤點狀態
/// </summary>
public enum StockCountStatus
{
    /// <summary>草稿</summary>
    Draft = 1,
    /// <summary>進行中</summary>
    InProgress = 2,
    /// <summary>已完成</summary>
    Completed = 3,
    /// <summary>已取消</summary>
    Cancelled = 4
}

/// <summary>
/// 庫存盤點
/// </summary>
public class StockCount
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 盤點單號
    /// </summary>
    public string CountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 倉庫編號
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 倉庫
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// 盤點日期
    /// </summary>
    public DateTime CountDate { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public StockCountStatus Status { get; set; } = StockCountStatus.Draft;

    /// <summary>
    /// 盤點範圍說明
    /// </summary>
    public string? Scope { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 總盤盈數量
    /// </summary>
    public int TotalSurplus { get; set; }

    /// <summary>
    /// 總盤虧數量
    /// </summary>
    public int TotalShortage { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 完成人員編號
    /// </summary>
    public int? CompletedBy { get; set; }

    /// <summary>
    /// 盤點明細
    /// </summary>
    public ICollection<StockCountItem> Items { get; set; } = new List<StockCountItem>();
}
