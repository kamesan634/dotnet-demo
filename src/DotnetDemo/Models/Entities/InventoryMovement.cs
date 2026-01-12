namespace DotnetDemo.Models.Entities;

/// <summary>
/// 庫存異動類型
/// </summary>
public enum MovementType
{
    /// <summary>入庫</summary>
    In = 1,
    /// <summary>出庫</summary>
    Out = 2,
    /// <summary>調撥入</summary>
    TransferIn = 3,
    /// <summary>調撥出</summary>
    TransferOut = 4,
    /// <summary>盤點調整</summary>
    Adjustment = 5
}

/// <summary>
/// 庫存異動記錄
/// </summary>
public class InventoryMovement
{
    /// <summary>
    /// 異動編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 倉庫編號
    /// </summary>
    public int WarehouseId { get; set; }

    /// <summary>
    /// 倉庫
    /// </summary>
    public Warehouse Warehouse { get; set; } = null!;

    /// <summary>
    /// 異動類型
    /// </summary>
    public MovementType Type { get; set; }

    /// <summary>
    /// 異動數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 異動前數量
    /// </summary>
    public int BeforeQuantity { get; set; }

    /// <summary>
    /// 異動後數量
    /// </summary>
    public int AfterQuantity { get; set; }

    /// <summary>
    /// 來源單據類型
    /// </summary>
    public string? ReferenceType { get; set; }

    /// <summary>
    /// 來源單據編號
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 操作人員編號
    /// </summary>
    public int CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
