namespace DotnetDemo.Models.Entities;

/// <summary>
/// 商品
/// </summary>
public class Product
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 商品代碼 (SKU)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 商品條碼
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 分類編號
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 分類
    /// </summary>
    public Category Category { get; set; } = null!;

    /// <summary>
    /// 單位編號
    /// </summary>
    public int UnitId { get; set; }

    /// <summary>
    /// 單位
    /// </summary>
    public Unit Unit { get; set; } = null!;

    /// <summary>
    /// 成本價
    /// </summary>
    public decimal CostPrice { get; set; }

    /// <summary>
    /// 售價
    /// </summary>
    public decimal SellingPrice { get; set; }

    /// <summary>
    /// 安全庫存量
    /// </summary>
    public int SafetyStock { get; set; }

    /// <summary>
    /// 預設供應商編號
    /// </summary>
    public int? DefaultSupplierId { get; set; }

    /// <summary>
    /// 預設供應商
    /// </summary>
    public Supplier? DefaultSupplier { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 庫存記錄
    /// </summary>
    public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    /// <summary>
    /// 供應商價格
    /// </summary>
    public ICollection<SupplierPrice> SupplierPrices { get; set; } = new List<SupplierPrice>();
}
