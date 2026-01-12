namespace DotnetDemo.Models.Entities;

/// <summary>
/// 調撥單狀態
/// </summary>
public enum StockTransferStatus
{
    /// <summary>待出庫</summary>
    Pending = 1,
    /// <summary>已出庫</summary>
    Shipped = 2,
    /// <summary>已入庫</summary>
    Received = 3,
    /// <summary>已取消</summary>
    Cancelled = 4
}

/// <summary>
/// 庫存調撥單
/// </summary>
public class StockTransfer
{
    /// <summary>
    /// 調撥單編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 調撥單號碼
    /// </summary>
    public string TransferNumber { get; set; } = string.Empty;

    /// <summary>
    /// 來源倉庫編號
    /// </summary>
    public int FromWarehouseId { get; set; }

    /// <summary>
    /// 來源倉庫
    /// </summary>
    public Warehouse FromWarehouse { get; set; } = null!;

    /// <summary>
    /// 目標倉庫編號
    /// </summary>
    public int ToWarehouseId { get; set; }

    /// <summary>
    /// 目標倉庫
    /// </summary>
    public Warehouse ToWarehouse { get; set; } = null!;

    /// <summary>
    /// 調撥狀態
    /// </summary>
    public StockTransferStatus Status { get; set; } = StockTransferStatus.Pending;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 出庫人員編號
    /// </summary>
    public int? ShippedBy { get; set; }

    /// <summary>
    /// 出庫時間
    /// </summary>
    public DateTime? ShippedAt { get; set; }

    /// <summary>
    /// 入庫人員編號
    /// </summary>
    public int? ReceivedBy { get; set; }

    /// <summary>
    /// 入庫時間
    /// </summary>
    public DateTime? ReceivedAt { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 調撥明細
    /// </summary>
    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}
