namespace DotnetDemo.Models.Entities;

/// <summary>
/// 採購退貨明細
/// </summary>
public class PurchaseReturnItem
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 退貨單編號
    /// </summary>
    public int PurchaseReturnId { get; set; }

    /// <summary>
    /// 退貨單
    /// </summary>
    public PurchaseReturn PurchaseReturn { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 退貨數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// 小計
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// 退貨原因
    /// </summary>
    public string? Reason { get; set; }
}
