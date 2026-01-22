namespace DotnetDemo.Models.Entities;

/// <summary>
/// 商品規格
/// </summary>
public class ProductVariant
{
    /// <summary>
    /// 編號
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
    /// 規格名稱 (例如: 顏色、尺寸)
    /// </summary>
    public string AttributeName { get; set; } = string.Empty;

    /// <summary>
    /// 規格值 (例如: 紅色、L號)
    /// </summary>
    public string AttributeValue { get; set; } = string.Empty;

    /// <summary>
    /// SKU (規格專屬代碼)
    /// </summary>
    public string? Sku { get; set; }

    /// <summary>
    /// 條碼
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// 成本價調整
    /// </summary>
    public decimal CostPriceAdjustment { get; set; }

    /// <summary>
    /// 售價調整
    /// </summary>
    public decimal SellingPriceAdjustment { get; set; }

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
}
