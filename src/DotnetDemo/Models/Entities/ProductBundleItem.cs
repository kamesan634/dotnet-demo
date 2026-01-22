namespace DotnetDemo.Models.Entities;

/// <summary>
/// 商品組合明細
/// </summary>
public class ProductBundleItem
{
    /// <summary>
    /// 編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 組合編號
    /// </summary>
    public int ProductBundleId { get; set; }

    /// <summary>
    /// 組合
    /// </summary>
    public ProductBundle ProductBundle { get; set; } = null!;

    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// 排序
    /// </summary>
    public int SortOrder { get; set; }
}
