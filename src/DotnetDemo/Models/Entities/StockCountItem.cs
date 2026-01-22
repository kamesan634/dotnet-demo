namespace DotnetDemo.Models.Entities;

/// <summary>
/// 盤點明細
/// </summary>
public class StockCountItem
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 盤點單編號
    /// </summary>
    public int StockCountId { get; set; }

    /// <summary>
    /// 盤點單
    /// </summary>
    public StockCount StockCount { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 系統庫存數量
    /// </summary>
    public int SystemQuantity { get; set; }

    /// <summary>
    /// 實際盤點數量
    /// </summary>
    public int? ActualQuantity { get; set; }

    /// <summary>
    /// 差異數量 (實際 - 系統)
    /// </summary>
    public int? Difference { get; set; }

    /// <summary>
    /// 差異原因
    /// </summary>
    public string? DifferenceReason { get; set; }

    /// <summary>
    /// 是否已盤點
    /// </summary>
    public bool IsCounted { get; set; }

    /// <summary>
    /// 盤點時間
    /// </summary>
    public DateTime? CountedAt { get; set; }

    /// <summary>
    /// 盤點人員編號
    /// </summary>
    public int? CountedBy { get; set; }
}
