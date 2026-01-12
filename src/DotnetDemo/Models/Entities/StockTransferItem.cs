namespace DotnetDemo.Models.Entities;

/// <summary>
/// 調撥明細
/// </summary>
public class StockTransferItem
{
    /// <summary>
    /// 明細編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 調撥單編號
    /// </summary>
    public int StockTransferId { get; set; }

    /// <summary>
    /// 調撥單
    /// </summary>
    public StockTransfer StockTransfer { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 調撥數量
    /// </summary>
    public int Quantity { get; set; }
}
