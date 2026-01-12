namespace DotnetDemo.Models.Entities;

/// <summary>
/// 進貨單
/// </summary>
public class PurchaseReceipt
{
    /// <summary>
    /// 進貨單編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 進貨單號碼
    /// </summary>
    public string ReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// 採購單編號
    /// </summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>
    /// 採購單
    /// </summary>
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>
    /// 倉庫編號
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 倉庫
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// 收貨日期
    /// </summary>
    public DateOnly ReceiptDate { get; set; }

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
    /// 進貨明細
    /// </summary>
    public ICollection<PurchaseReceiptItem> Items { get; set; } = new List<PurchaseReceiptItem>();
}
