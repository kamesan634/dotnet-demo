namespace DotnetDemo.Models.Entities;

/// <summary>
/// 供應商商品價格
/// </summary>
public class SupplierPrice
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 供應商編號
    /// </summary>
    public int SupplierId { get; set; }

    /// <summary>
    /// 供應商
    /// </summary>
    public Supplier Supplier { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 供應商商品代碼
    /// </summary>
    public string? SupplierProductCode { get; set; }

    /// <summary>
    /// 採購價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 最小訂購量
    /// </summary>
    public int MinOrderQuantity { get; set; } = 1;

    /// <summary>
    /// 前置時間 (天)
    /// </summary>
    public int LeadTimeDays { get; set; } = 7;

    /// <summary>
    /// 是否為優先供應商
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// 生效日期
    /// </summary>
    public DateOnly EffectiveDate { get; set; }

    /// <summary>
    /// 失效日期
    /// </summary>
    public DateOnly? ExpiryDate { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
