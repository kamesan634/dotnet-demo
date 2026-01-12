namespace DotnetDemo.Models.Entities;

/// <summary>
/// 採購單明細
/// </summary>
public class PurchaseOrderItem
{
    /// <summary>
    /// 明細編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 採購單編號
    /// </summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>
    /// 採購單
    /// </summary>
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 採購數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 已收貨數量
    /// </summary>
    public int ReceivedQuantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    public decimal SubTotal { get; set; }
}
