namespace DotnetDemo.Models.Entities;

/// <summary>
/// 採購單狀態
/// </summary>
public enum PurchaseOrderStatus
{
    /// <summary>草稿</summary>
    Draft = 1,
    /// <summary>待核准</summary>
    PendingApproval = 2,
    /// <summary>已核准</summary>
    Approved = 3,
    /// <summary>部分收貨</summary>
    PartialReceived = 4,
    /// <summary>已完成</summary>
    Completed = 5,
    /// <summary>已取消</summary>
    Cancelled = 6
}

/// <summary>
/// 採購單
/// </summary>
public class PurchaseOrder
{
    /// <summary>
    /// 採購單編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 採購單號碼
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 供應商編號
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// 供應商
    /// </summary>
    public Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// 目標倉庫編號
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 目標倉庫
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// 採購單狀態
    /// </summary>
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;

    /// <summary>
    /// 預計交貨日期
    /// </summary>
    public DateOnly ExpectedDeliveryDate { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 核准人員編號
    /// </summary>
    public int? ApprovedBy { get; set; }

    /// <summary>
    /// 核准時間
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 採購單明細
    /// </summary>
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

    /// <summary>
    /// 進貨單
    /// </summary>
    public ICollection<PurchaseReceipt> Receipts { get; set; } = new List<PurchaseReceipt>();
}
