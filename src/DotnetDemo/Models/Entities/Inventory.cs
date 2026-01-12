namespace DotnetDemo.Models.Entities;

/// <summary>
/// 庫存
/// </summary>
public class Inventory
{
    /// <summary>
    /// 庫存編號
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
    /// 庫存數量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 預留數量
    /// </summary>
    public int ReservedQuantity { get; set; }

    /// <summary>
    /// 可用數量
    /// </summary>
    public int AvailableQuantity => Quantity - ReservedQuantity;

    /// <summary>
    /// 最後更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
