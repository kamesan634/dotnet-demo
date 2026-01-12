namespace DotnetDemo.Models.Entities;

/// <summary>
/// 進貨單明細
/// </summary>
public class PurchaseReceiptItem
{
    /// <summary>
    /// 明細編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 進貨單編號
    /// </summary>
    public int PurchaseReceiptId { get; set; }

    /// <summary>
    /// 進貨單
    /// </summary>
    public PurchaseReceipt PurchaseReceipt { get; set; } = null!;

    /// <summary>
    /// 採購單明細編號
    /// </summary>
    public int PurchaseOrderItemId { get; set; }

    /// <summary>
    /// 採購單明細
    /// </summary>
    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 收貨數量
    /// </summary>
    public int Quantity { get; set; }
}
