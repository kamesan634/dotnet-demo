namespace DotnetDemo.Models.Entities;

/// <summary>
/// 採購退貨狀態
/// </summary>
public enum PurchaseReturnStatus
{
    /// <summary>待處理</summary>
    Pending = 1,
    /// <summary>已確認</summary>
    Confirmed = 2,
    /// <summary>已退貨</summary>
    Returned = 3,
    /// <summary>已取消</summary>
    Cancelled = 4
}

/// <summary>
/// 採購退貨
/// </summary>
public class PurchaseReturn
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 退貨單號
    /// </summary>
    public string ReturnNumber { get; set; } = string.Empty;

    /// <summary>
    /// 進貨單編號 (可選，關聯原進貨單)
    /// </summary>
    public int? PurchaseReceiptId { get; set; }

    /// <summary>
    /// 進貨單
    /// </summary>
    public PurchaseReceipt? PurchaseReceipt { get; set; }

    /// <summary>
    /// 供應商編號
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// 供應商
    /// </summary>
    public Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// 倉庫編號
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 倉庫
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// 退貨原因
    /// </summary>
    public string Reason { get; set; } = string.Empty;

    /// <summary>
    /// 退貨總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public PurchaseReturnStatus Status { get; set; } = PurchaseReturnStatus.Pending;

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
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 退貨明細
    /// </summary>
    public ICollection<PurchaseReturnItem> Items { get; set; } = new List<PurchaseReturnItem>();
}
