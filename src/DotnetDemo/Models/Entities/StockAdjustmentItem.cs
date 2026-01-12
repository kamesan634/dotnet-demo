namespace DotnetDemo.Models.Entities;

/// <summary>
/// 庫存調整明細
/// </summary>
public class StockAdjustmentItem
{
    /// <summary>
    /// 明細編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 調整單編號
    /// </summary>
    public int StockAdjustmentId { get; set; }

    /// <summary>
    /// 調整單
    /// </summary>
    public StockAdjustment StockAdjustment { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 調整前數量
    /// </summary>
    public int BeforeQuantity { get; set; }

    /// <summary>
    /// 調整後數量
    /// </summary>
    public int AfterQuantity { get; set; }

    /// <summary>
    /// 調整數量
    /// </summary>
    public int AdjustmentQuantity { get; set; }
}
