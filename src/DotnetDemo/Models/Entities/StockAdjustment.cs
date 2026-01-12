namespace DotnetDemo.Models.Entities;

/// <summary>
/// 庫存調整單
/// </summary>
public class StockAdjustment
{
    /// <summary>
    /// 調整單編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 調整單號碼
    /// </summary>
    public string AdjustmentNumber { get; set; } = string.Empty;

    /// <summary>
    /// 倉庫編號
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 倉庫
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// 調整原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 調整明細
    /// </summary>
    public ICollection<StockAdjustmentItem> Items { get; set; } = new List<StockAdjustmentItem>();
}
