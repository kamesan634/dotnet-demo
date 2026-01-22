namespace DotnetDemo.Models.Entities;

/// <summary>
/// 商品組合
/// </summary>
public class ProductBundle
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 組合代碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 組合名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 說明
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 組合售價
    /// </summary>
    public decimal SellingPrice { get; set; }

    /// <summary>
    /// 原價總計 (各單品售價總和)
    /// </summary>
    public decimal OriginalTotal { get; set; }

    /// <summary>
    /// 開始銷售日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束銷售日期
    /// </summary>
    public DateTime? EndDate { get; set; }

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
    /// 組合明細
    /// </summary>
    public ICollection<ProductBundleItem> Items { get; set; } = new List<ProductBundleItem>();
}
